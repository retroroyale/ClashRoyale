using System.Threading.Tasks;
using ClashRoyale.Core.Cluster.Protocol;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace ClashRoyale.Core.Network.Handlers.Cluster
{
    public class ClusterPacketEncoder : ChannelHandlerAdapter
    {
        public override Task WriteAsync(IChannelHandlerContext context, object msg)
        {
            if (!(msg is ClusterMessage message)) return base.WriteAsync(context, null);

            message.Encode();
            message.Encrypt();

            var header = Unpooled.Buffer(5);
            header.WriteUnsignedShort(message.Id);
            header.WriteMedium(message.Length);

            base.WriteAsync(context, header);

            return base.WriteAsync(context, message.Writer);
        }
    }
}