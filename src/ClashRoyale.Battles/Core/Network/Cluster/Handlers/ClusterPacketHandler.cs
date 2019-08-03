using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using SharpRaven.Data;

namespace ClashRoyale.Battles.Core.Network.Cluster.Handlers
{
    public class ClusterPacketHandler : ChannelHandlerAdapter
    {
        public IChannel Channel { get; set; }

        public ClusterPacketHandler()
        {
            Resources.ClusterClient.Handler = this;
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var buffer = (IByteBuffer) message;
            if (buffer == null) return;

            Resources.ClusterClient.Process(buffer);
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        public override void ChannelRegistered(IChannelHandlerContext context)
        {
            Channel = context.Channel;

            base.ChannelRegistered(context);
        }

        public override async void ChannelUnregistered(IChannelHandlerContext context)
        {
            Logger.Log($"Disconnected from {Channel.RemoteAddress}. Retrying in 5sec.", GetType(), ErrorLevel.Debug);

            await Task.Delay(5000);

            await Task.Run(Resources.NettyClient.RunClientAsync);

            base.ChannelUnregistered(context);
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            if (exception.GetType() != typeof(ReadTimeoutException) &&
                exception.GetType() != typeof(WriteTimeoutException) &&
                exception.GetType() != typeof(SocketException))
                Logger.Log(exception, GetType(), ErrorLevel.Error);

            context.CloseAsync();
        }
    }
}