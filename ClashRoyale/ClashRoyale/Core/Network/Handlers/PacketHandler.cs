using System;
using ClashRoyale.Logic;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using SharpRaven.Data;

namespace ClashRoyale.Core.Network.Handlers
{
    public class PacketHandler : ChannelHandlerAdapter
    {
        public PacketHandler()
        {
            Device = new Device(this);
        }

        public Device Device { get; set; }
        public IChannel Channel { get; set; }

        public override async void ChannelRead(IChannelHandlerContext context, object message)
        {
            var buffer = (IByteBuffer) message;
            if (buffer == null) return;

            var packet = new byte[buffer.ReadableBytes];
            buffer.GetBytes(buffer.ReaderIndex, packet);

            await Device.Process(Unpooled.WrappedBuffer(packet));
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        public override void ChannelRegistered(IChannelHandlerContext context)
        {
            Channel = context.Channel;

            Logger.Log($"Client {Channel.RemoteAddress} connected.", GetType(), ErrorLevel.Debug);

            base.ChannelRegistered(context);
        }

        public override void ChannelUnregistered(IChannelHandlerContext context)
        {
            Logger.Log($"Client {Channel.RemoteAddress} disconnected.", GetType(), ErrorLevel.Debug);

            if(Device?.Player?.Home != null)
                Resources.Players.Logout(Device.Player.Home.PlayerId);

            base.ChannelUnregistered(context);
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Logger.Log(exception, GetType(), ErrorLevel.Error);
            context.CloseAsync();
        }
    }
}