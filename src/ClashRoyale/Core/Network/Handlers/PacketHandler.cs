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

            await Device.ProcessAsync(Unpooled.CopiedBuffer(packet));
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
                    var alliance = await Resources.Alliances.GetAllianceAsync(player.Home.AllianceInfo.Id);
                    if (alliance != null)
                    {
                        var entry = alliance.Stream.Find(e => e.SenderId == player.Home.Id && e.StreamEntryType == 10);
                        if (entry != null)
                        {
                            alliance.RemoveEntry(entry);
                        }

                        if (alliance.Online < 1)
                        {
                            Resources.Alliances.Remove(alliance.Id);
                            Logger.Log($"Uncached Clan {alliance.Id} because no member is online.", GetType(),
                                ErrorLevel.Debug);
                        }
                        else
                        {
                            alliance.UpdateOnlineCount();
                        }
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