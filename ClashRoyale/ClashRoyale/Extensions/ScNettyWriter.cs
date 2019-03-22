using System;
using System.Linq;
using System.Text;
using ClashRoyale.Files.CsvHelpers;
using DotNetty.Buffers;

namespace ClashRoyale.Extensions
{
    /// <summary>
    ///     This implements a few extensions for games from supercell
    /// </summary>
    public static class ScNettyWriter
    {
        /// <summary>
        ///     Encodes a string based on the length
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        public static void WriteScString(this IByteBuffer buffer, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                buffer.WriteInt(0);
            }
            else
            {
                var bytes = Encoding.UTF8.GetBytes(value);

                buffer.WriteInt(bytes.Length);
                buffer.WriteString(value, Encoding.UTF8);
            }
        }

        /// <summary>
        ///     Encodes a VInt
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        public static void WriteVInt(this IByteBuffer buffer, int value)
        {
            try
            {
                if (value > 0x3F)
                {
                    buffer.WriteByte((byte) ((value & 0x3F) | 0x80));
                    if (value > 0x1FFF)
                    {
                        buffer.WriteByte((byte) ((value >> 0x6) | 0x80));
                        if (value > 0xFFFFF)
                        {
                            buffer.WriteByte((byte) ((value >> 0xD) | 0x80));
                            if (value > 0x7FFFFFF)
                            {
                                buffer.WriteByte((byte) ((value >> 0x14) | 0x80));
                                value >>= 0x1B & 0x7F;
                            }
                            else
                            {
                                value >>= 0x14 & 0x7F;
                            }
                        }
                        else
                        {
                            value >>= 0xD & 0x7F;
                        }
                    }
                    else
                    {
                        value >>= 0x6 & 0x7F;
                    }
                }

                buffer.WriteByte((byte) value);
            }
            catch (IndexOutOfRangeException)
            {
                // Ignored.
            }
        }

        /// <summary>
        ///     Encodes a "NullVInt"
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="count"></param>
        public static void WriteNullVInt(this IByteBuffer buffer, int count = 1)
        {
            for (var i = 0; i < count; i++)
                buffer.WriteByte(0x7F);
        }

        /// <summary>
        ///     Encodes CsvData
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        public static void WriteData(this IByteBuffer buffer, Data value)
        {
            buffer.WriteVInt(value.GetDataType() + 31);
            buffer.WriteVInt(value.GetInstanceId());
        }

        /// <summary>
        ///     Encodes raw bytes to the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="bytes"></param>
        public static void WriteBuffer(this IByteBuffer buffer, byte[] bytes)
        {
            buffer.WriteBytes(Unpooled.WrappedBuffer(bytes));
        }

        /// <summary>
        ///     This method should be only used for testing.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        public static void WriteHex(this IByteBuffer buffer, string value)
        {
            var tmp = value.Replace("-", string.Empty);
            buffer.WriteBuffer(Enumerable.Range(0, tmp.Length).Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(tmp.Substring(x, 2), 16)).ToArray());
        }
    }
}