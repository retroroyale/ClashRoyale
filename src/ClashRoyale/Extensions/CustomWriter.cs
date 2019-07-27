using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;

namespace ClashRoyale.Extensions
{
    /// <summary>
    ///     This implements a few extensions for games from supercell
    /// </summary>
    public static class CustomWriter
    {
        /// <summary>
        ///     Encodes CsvData
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        public static void WriteData(this IByteBuffer buffer, Data value)
        {
            buffer.WriteVInt(value.GetDataType());
            buffer.WriteVInt(value.GetInstanceId());
        }
    }
}