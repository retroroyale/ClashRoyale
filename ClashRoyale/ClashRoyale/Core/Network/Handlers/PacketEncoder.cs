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
            var message = (PiranhaMessage)msg;

            if (message != null)
            {
                message.Encode();
                message.Encrypt();

                var buffer = Unpooled.Buffer();

                buffer.WriteUnsignedShort(message.Id);
                buffer.WriteMedium(message.Length);
                buffer.WriteUnsignedShort(message.Version);

                buffer.WriteBytes(message.Packet);

                return base.WriteAsync(context, buffer);
            }

            return base.WriteAsync(context, message);
        }
    }
}