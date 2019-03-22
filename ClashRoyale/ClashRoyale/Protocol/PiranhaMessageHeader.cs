using DotNetty.Buffers;

namespace ClashRoyale.Protocol
{
    public class PiranhaMessageHeader
    {
        public IByteBuffer Buffer = Unpooled.Buffer();
        public ushort Id { get; set; }
        public int Length { get; set; }
        public ushort Version { get; set; }
    }
}