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
            #region ChannelRead

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

            #endregion 
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

        public override async void ChannelUnregistered(IChannelHandlerContext context)
        {          
            if (Device?.Player?.Home != null)
            {
                var player = Device.Player;

                Resources.Players.Logout(player.Home.Id);

                if (player.Home.AllianceInfo.HasAlliance)
                {
                    var alliance = await Resources.Alliances.GetAlliance(player.Home.AllianceInfo.Id);
                    if (alliance != null)
                    {
                        if (alliance.Online < 1)
                        {
                            Resources.Alliances.Remove(alliance.Id);
                            Logger.Log($"Uncached Clan {alliance.Id} because no member is online.", GetType(), ErrorLevel.Debug);
                        }
                        else
                            alliance.UpdateOnlineCount();
                    }
                }              
            }

            Logger.Log($"Client {Channel.RemoteAddress} disconnected.", GetType(), ErrorLevel.Debug);

            base.ChannelUnregistered(context);
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Logger.Log(exception, GetType(), ErrorLevel.Error);
            context.CloseAsync();
        }
    }
}