using System;
using System.Net.Sockets;
using DotNetty.Buffers;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using SharpRaven.Data;

namespace ClashRoyale.Battles.Core.Network.Cluster.Handlers
{
    public class ClusterPacketHandler : ChannelHandlerAdapter
    {
        public IChannel Channel { get; set; }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var buffer = (IByteBuffer) message;
            if (buffer == null) return;

            ClusterClient.Process(buffer);
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        public override void ChannelRegistered(IChannelHandlerContext context)
        {
            Channel = context.Channel;

            if (!Channel.Active)
                return;

            Logger.Log($"Connected to {Channel.RemoteAddress}", GetType(), ErrorLevel.Debug);

            // TODO SEND CONNECTION CHECK MESSAGE

            base.ChannelRegistered(context);
        }

        public override void ChannelUnregistered(IChannelHandlerContext context)
        {
            Logger.Log($"Disconnected from {Channel.RemoteAddress}", GetType(), ErrorLevel.Debug);

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