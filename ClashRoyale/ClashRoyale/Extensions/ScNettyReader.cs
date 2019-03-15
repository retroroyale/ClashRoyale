using System;
using System.Text;
using DotNetty.Buffers;

namespace ClashRoyale.Extensions
{
    /// <summary>
    ///     This implements a few extensions for games from supercell
    /// </summary>
    public static class ScNettyReader
    {
        /// <summary>
        ///     Decodes a string based on the length
        /// </summary>
        /// <param name="byteBuffer"></param>
        /// <returns></returns>
        public static string ReadScString(this IByteBuffer byteBuffer)
        {
            var length = byteBuffer.ReadInt();

            if (length <= 0 || length > 900000)
                return string.Empty;

            return byteBuffer.ReadString(length, Encoding.UTF8);
        }

        /// <summary>
        ///     Decodes a VInt
        /// </summary>
        /// <param name="byteBuffer"></param>
        /// <returns></returns>
        public static int ReadVInt(this IByteBuffer byteBuffer)
        {
            try
            {
                var b = byteBuffer.ReadByte();
                var v5 = b & 0x80;
                var lr = b & 0x3F;

                if ((b & 0x40) != 0)
                {
                    if (v5 == 0)
                        return lr;

                    b = byteBuffer.ReadByte();
                    v5 = ((b << 6) & 0x1FC0) | lr;
                    if ((b & 0x80) != 0)
                    {
                        b = byteBuffer.ReadByte();
                        v5 = v5 | ((b << 13) & 0xFE000);
                        if ((b & 0x80) != 0)
                        {
                            b = byteBuffer.ReadByte();
                            v5 = v5 | ((b << 20) & 0x7F00000);
                            if ((b & 0x80) != 0)
                            {
                                b = byteBuffer.ReadByte();
                                lr = (int) (v5 | (b << 27) | 0x80000000);
                            }
                            else
                            {
                                lr = (int) (v5 | 0xF8000000);
                            }
                        }
                        else
                        {
                            lr = (int) (v5 | 0xFFF00000);
                        }
                    }
                    else
                    {
                        lr = (int) (v5 | 0xFFFFE000);
                    }
                }
                else
                {
                    if (v5 == 0)
                        return lr;

                    b = byteBuffer.ReadByte();
                    lr |= (b << 6) & 0x1FC0;
                    if ((b & 0x80) == 0)
                        return lr;

                    b = byteBuffer.ReadByte();
                    lr |= (b << 13) & 0xFE000;
                    if ((b & 0x80) == 0)
                        return lr;

                    b = byteBuffer.ReadByte();
                    lr |= (b << 20) & 0x7F00000;
                    if ((b & 0x80) == 0)
                        return lr;

                    b = byteBuffer.ReadByte();
                    lr |= b << 27;
                }

                return lr;
            }
            catch (IndexOutOfRangeException)
            {
                return -1;
            }
        }
    }
}