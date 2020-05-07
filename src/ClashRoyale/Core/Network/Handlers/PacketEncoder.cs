using System.Threading.Tasks;
using ClashRoyale.Protocol;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace ClashRoyale.Core.Network.Handlers
{
    public class PacketEncoder : ChannelHandlerAdapter
    {
        public override Task WriteAsync(IChannelHandlerContext context, object msg)
        {
            if (!(msg is PiranhaMessage message)) return base.WriteAsync(context, null);

            message.Encode();
            message.Encrypt();

            var header = PooledByteBufferAllocator.Default.Buffer(7);
            header.WriteUnsignedShort(message.Id);
            header.WriteMedium(message.Writer.WriterIndex);
            header.WriteUnsignedShort(message.Version);

            base.WriteAsync(context, header);

            return base.WriteAsync(context, message.Writer);
        }
    }
}