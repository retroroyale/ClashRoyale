namespace SevenZip.Sdk.Compression.RangeCoder
{
    internal struct BitEncoder
    {
        public const uint kBitModelTotal = 1 << kNumBitModelTotalBits;
        public const int kNumBitModelTotalBits = 11;
        public const int kNumBitPriceShiftBits = 6;
        private const int kNumMoveBits = 5;
        private const int kNumMoveReducingBits = 2;
        private static readonly uint[] ProbPrices = new uint[kBitModelTotal >> kNumMoveReducingBits];

        private uint Prob;

        static BitEncoder()
        {
            const int kNumBits = kNumBitModelTotalBits - kNumMoveReducingBits;
            for (var i = kNumBits - 1; i >= 0; i--)
            {
                var start = (uint) 1 << (kNumBits - i - 1);
                var end = (uint) 1 << (kNumBits - i);
                for (var j = start; j < end; j++)
                    ProbPrices[j] = ((uint) i << kNumBitPriceShiftBits) +
                                    (((end - j) << kNumBitPriceShiftBits) >> (kNumBits - i - 1));
            }
        }

        public void Init()
        {
            Prob = kBitModelTotal >> 1;
        }

        public void Encode(Encoder encoder, uint symbol)
        {
            var newBound = (encoder.Range >> kNumBitModelTotalBits) * Prob;
            if (symbol == 0)
            {
                encoder.Range = newBound;
                Prob += (kBitModelTotal - Prob) >> kNumMoveBits;
            }
            else
            {
                encoder.Low += newBound;
                encoder.Range -= newBound;
                Prob -= Prob >> kNumMoveBits;
            }

            if (encoder.Range < Encoder.kTopValue)
            {
                encoder.Range <<= 8;
                encoder.ShiftLow();
            }
        }

        public uint GetPrice(uint symbol)
        {
            return ProbPrices[(((Prob - symbol) ^ -(int) symbol) & (kBitModelTotal - 1)) >> kNumMoveReducingBits];
        }

        public uint GetPrice0()
        {
            return ProbPrices[Prob >> kNumMoveReducingBits];
        }

        public uint GetPrice1()
        {
            return ProbPrices[(kBitModelTotal - Prob) >> kNumMoveReducingBits];
        }
    }

    internal struct BitDecoder
    {
        public const uint kBitModelTotal = 1 << kNumBitModelTotalBits;
        public const int kNumBitModelTotalBits = 11;
        private const int kNumMoveBits = 5;

        private uint Prob;

        public void Init()
        {
            Prob = kBitModelTotal >> 1;
        }

        public uint Decode(Decoder rangeDecoder)
        {
            var newBound = (rangeDecoder.Range >> kNumBitModelTotalBits) * Prob;
            if (rangeDecoder.Code < newBound)
            {
                rangeDecoder.Range = newBound;
                Prob += (kBitModelTotal - Prob) >> kNumMoveBits;
                if (rangeDecoder.Range < Decoder.kTopValue)
                {
                    rangeDecoder.Code = (rangeDecoder.Code << 8) | (byte) rangeDecoder.Stream.ReadByte();
                    rangeDecoder.Range <<= 8;
                }

                return 0;
            }

            rangeDecoder.Range -= newBound;
            rangeDecoder.Code -= newBound;
            Prob -= Prob >> kNumMoveBits;
            if (rangeDecoder.Range < Decoder.kTopValue)
            {
                rangeDecoder.Code = (rangeDecoder.Code << 8) | (byte) rangeDecoder.Stream.ReadByte();
                rangeDecoder.Range <<= 8;
            }

            return 1;
        }
    }
}