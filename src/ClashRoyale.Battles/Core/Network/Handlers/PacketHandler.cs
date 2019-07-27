using System;
using ClashRoyale.Utilities.Netty;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using SharpRaven.Data;

namespace ClashRoyale.Battles.Core.Network.Handlers
{
    public class PacketHandler : SimpleChannelInboundHandler<DatagramPacket>
    {
        public IChannel Channel { get; set; }

        protected override void ChannelRead0(IChannelHandlerContext ctx, DatagramPacket packet)
        {
            Logger.Log($"Received {packet.Content.ReadableBytes} bytes", GetType(), ErrorLevel.Debug);

            packet.Content.ReadBytes(10); // Session Id
            Logger.Log($"ACK COUNT: {packet.Content.ReadVInt()}", GetType(), ErrorLevel.Debug);
            Logger.Log($"CHUNK COUNT: {packet.Content.ReadVInt()}", GetType(), ErrorLevel.Debug);
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            if (exception.GetType() != typeof(ReadTimeoutException) &&
                exception.GetType() != typeof(WriteTimeoutException))
                Logger.Log(exception, GetType(), ErrorLevel.Error);

            context.CloseAsync();
        }
    }
}