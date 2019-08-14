using ClashRoyale.Battles.Logic.Session;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace ClashRoyale.Battles.Logic
{
    public class UdpMessageProcessor
    {
        public static async void Process(IChannelHandlerContext ctx, DatagramPacket packet)
        {
            var content = packet.Content;
            var length = content.ReadableBytes;

            if (length == 1400)
            {
                var sessionId = content.ReadLong();
                var gameMode = content.ReadByte();
                var team = content.ReadByte();

                var sessionBuffer = Unpooled.Buffer();
                sessionBuffer.WriteLong(sessionId);
                sessionBuffer.WriteByte(gameMode);
                sessionBuffer.WriteByte(team);

                Resources.Sessions.Add(new SessionContext
                {
                    EndPoint = packet.Sender,
                    GameMode = gameMode,
                    Index = team
                }, sessionId);

                await ctx.WriteAsync(new DatagramPacket(sessionBuffer, packet.Sender));
            }
            else
            {
                //if (length != 10)
                //Logger.Log($"Received {length} bytes from {packet.Sender}: {BitConverter.ToString(content.Array.Take(length).ToArray()).Replace("-", "")}", null, ErrorLevel.Debug);

                var sessionId = content.ReadLong();
                var gameMode = content.ReadByte();
                var team = content.ReadByte();

                var session = Resources.Sessions.Get(sessionId);
                if (session == null) return;

                var sessionContext = session.Get(packet.Sender);

                if (sessionContext != null)
                {
                    sessionContext.Active = true;

                    if (length != 10)
                    {
                        sessionContext.Process(content, ctx.Channel);
                    }
                    else
                    {
                        var sessionOkBuffer = Unpooled.Buffer(10);
                        sessionOkBuffer.WriteLong(session.Id);
                        sessionOkBuffer.WriteByte(gameMode);
                        sessionOkBuffer.WriteByte(team);

                        await ctx.WriteAsync(new DatagramPacket(sessionOkBuffer, packet.Sender));
                    }
                }
            }
        }
    }
}