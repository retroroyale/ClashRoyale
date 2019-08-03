using System;
using System.Net;
using System.Net.Sockets;
using ClashRoyale.Core.Cluster;
using DotNetty.Buffers;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using SharpRaven.Data;

namespace ClashRoyale.Core.Network.Handlers.Cluster
{
    public class ClusterPacketHandler : ChannelHandlerAdapter
    {
        public IChannel Channel { get; set; }
        public Server Server { get; set; }

        public ClusterPacketHandler()
        {
            Server = new Server(this);
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var buffer = (IByteBuffer) message;
            if (buffer == null) return;

            Server.Process(buffer);
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        public override void ChannelRegistered(IChannelHandlerContext context)
        {
            Channel = context.Channel;

            var remoteAddress = (IPEndPoint)Channel.RemoteAddress;

            Logger.Log($"Server {remoteAddress.Address.MapToIPv4()}:{remoteAddress.Port} connected.", GetType(), ErrorLevel.Debug);

            base.ChannelRegistered(context);
        }

        public override void ChannelUnregistered(IChannelHandlerContext context)
        {
            var remoteAddress = (IPEndPoint)Channel.RemoteAddress;

            Logger.Log($"Server {remoteAddress.Address.MapToIPv4()}:{remoteAddress.Port} disconnected.", GetType(), ErrorLevel.Debug);

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