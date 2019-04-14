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
        /// Decodes a VInt (Variable Length Integer) - special greets to nameless who made this way smaller
        /// </summary>
        /// <param name="byteBuffer"></param>
        /// <returns></returns>
        public static int ReadVInt(this IByteBuffer byteBuffer)
        {
            byte b;
            var sign = ((b = byteBuffer.ReadByte()) >> 6) & 1;
            var i = b & 0x3F;

            for (;;)
            {
                if ((b & 0x80) == 0) break;
                i |= ((b = byteBuffer.ReadByte()) & 0x7F) << 6;
                if ((b & 0x80) == 0) break;
                i |= ((b = byteBuffer.ReadByte()) & 0x7F) << (6 + 7);
                if ((b & 0x80) == 0) break;
                i |= ((b = byteBuffer.ReadByte()) & 0x7F) << (6 + 7 + 7);
                if ((b & 0x80) == 0) break;
                i |= ((b = byteBuffer.ReadByte()) & 0x7F) << (6 + 7 + 7 + 7);
                if ((b & 0x80) == 0) break;
                return -1;
            }

            i ^= -sign;
            return i;
        }
    }
}