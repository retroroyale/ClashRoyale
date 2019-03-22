using System;
using ClashRoyale.Logic;
using ClashRoyale.Protocol;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using SharpRaven.Data;

namespace ClashRoyale.Core.Network.Handlers
{
    public class PacketHandler : ChannelHandlerAdapter
    {
        private bool _hasHeader;
        private PiranhaMessageHeader _messageHeader;

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

            while (buffer.ReadableBytes > 0)
                if (!_hasHeader)
                {
                    _messageHeader = new PiranhaMessageHeader
                    {
                        Id = buffer.ReadUnsignedShort(),
                        Length = buffer.ReadMedium(),
                        Version = buffer.ReadUnsignedShort()
                    };

                    _hasHeader = true;

                    var readableBytes = buffer.ReadableBytes;

                    if (_messageHeader.Length == readableBytes)
                    {
                        _messageHeader.Buffer.WriteBytes(buffer, readableBytes);

                        await Device.Process(_messageHeader);
                        _hasHeader = false;
                    }
                    else if (_messageHeader.Length < readableBytes)
                    {
                        _messageHeader.Buffer.WriteBytes(buffer, _messageHeader.Length);

                        await Device.Process(_messageHeader);
                        _hasHeader = false;
                    }
                    else
                    {
                        _messageHeader.Buffer.WriteBytes(buffer, readableBytes);
                    }
                }
                else
                {
                    _messageHeader.Buffer.WriteBytes(buffer, buffer.ReadableBytes);

                    if (_messageHeader.Buffer.ReadableBytes != _messageHeader.Length) continue;

                    await Device.Process(_messageHeader);
                    _hasHeader = false;
                }
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

            if (Device?.Player?.Home != null)
                Resources.Players.Logout(Device.Player.Home.Id);

            base.ChannelUnregistered(context);
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Logger.Log(exception, GetType(), ErrorLevel.Error);
            context.CloseAsync();
        }
    }
}