using System;
using System.IO;
using SevenZip;
using LZMAEncoder = SevenZip.Compression.LZMA.Encoder;

namespace ClashRoyale.Extensions.Utils
{
    public class CompressionUtils
    {
        public static byte[] CompressData(byte[] input)
        {
            var encoder = new LZMAEncoder();

            using (var uncompressed = new MemoryStream(input))
            {
                using (var compressed = new MemoryStream())
                {
                    encoder.SetCoderProperties(new[]
                    {
                        CoderPropID.DictionarySize,
                        CoderPropID.PosStateBits,
                        CoderPropID.LitContextBits,
                        CoderPropID.LitPosBits,
                        CoderPropID.Algorithm,
                        CoderPropID.NumFastBytes,
                        CoderPropID.MatchFinder,
                        CoderPropID.EndMarker
                    }, new object[]
                    {
                        262144,
                        2,
                        3,
                        0,
                        2,
                        32,
                        "bt4",
                        false
                    });

                    encoder.WriteCoderProperties(compressed);

                    compressed.Write(BitConverter.GetBytes(uncompressed.Length), 0, 4);

                    encoder.Code(uncompressed, compressed, uncompressed.Length, -1L,
                        null);

                    return compressed.ToArray();
                }
            }
        }
    }
}