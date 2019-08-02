using System.Threading.Tasks;
using ClashRoyale.Battles.Core.Network.Cluster.Protocol;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace ClashRoyale.Battles.Core.Network.Cluster.Handlers
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