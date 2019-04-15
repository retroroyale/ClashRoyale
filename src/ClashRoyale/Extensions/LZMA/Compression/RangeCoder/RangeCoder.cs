using System.IO;

namespace SevenZip.Sdk.Compression.RangeCoder
{
    internal class Encoder
    {
        public const uint kTopValue = 1 << 24;
        private byte _cache;
        private uint _cacheSize;

        public ulong Low;
        public uint Range;

        private long StartPosition;
        private Stream Stream;

        public void SetStream(Stream stream)
        {
            Stream = stream;
        }

        public void ReleaseStream()
        {
            Stream = null;
        }

        public void Init()
        {
            StartPosition = Stream.Position;

            Low = 0;
            Range = 0xFFFFFFFF;
            _cacheSize = 1;
            _cache = 0;
        }

        public void FlushData()
        {
            for (var i = 0; i < 5; i++)
                ShiftLow();
        }

        public void FlushStream()
        {
            Stream.Flush();
        }

        public void ShiftLow()
        {
            if ((uint) Low < 0xFF000000 || (uint) (Low >> 32) == 1)
            {
                var temp = _cache;
                do
                {
                    Stream.WriteByte((byte) (temp + (Low >> 32)));
                    temp = 0xFF;
                } while (--_cacheSize != 0);

                _cache = (byte) ((uint) Low >> 24);
            }

            _cacheSize++;
            Low = (uint) Low << 8;
        }

        public void EncodeDirectBits(uint v, int numTotalBits)
        {
            for (var i = numTotalBits - 1; i >= 0; i--)
            {
                Range >>= 1;
                if (((v >> i) & 1) == 1)
                    Low += Range;
                if (Range < kTopValue)
                {
                    Range <<= 8;
                    ShiftLow();
                }
            }
        }

        public long GetProcessedSizeAdd()
        {
            return _cacheSize +
                   Stream.Position - StartPosition + 4;
        }
    }

    internal class Decoder
    {
        public const uint kTopValue = 1 << 24;
        public uint Code;

        public uint Range;

        public Stream Stream;

        public void Init(Stream stream)
        {
            Stream = stream;

            Code = 0;
            Range = 0xFFFFFFFF;
            for (var i = 0; i < 5; i++)
                Code = (Code << 8) | (byte) Stream.ReadByte();
        }

        public void ReleaseStream()
        {
            Stream = null;
        }

        public uint DecodeDirectBits(int numTotalBits)
        {
            var range = Range;
            var code = Code;
            uint result = 0;
            for (var i = numTotalBits; i > 0; i--)
            {
                range >>= 1;
                var t = (code - range) >> 31;
                code -= range & (t - 1);
                result = (result << 1) | (1 - t);

                if (range < kTopValue)
                {
                    code = (code << 8) | (byte) Stream.ReadByte();
                    range <<= 8;
                }
            }

            Range = range;
            Code = code;
            return result;
        }
    }
}