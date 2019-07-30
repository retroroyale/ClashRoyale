using System;
using System.Linq;
using ClashRoyale.Battles.Logic.Session;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using SharpRaven.Data;

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
                var uselessBytes = content.ReadBytes(2);

                var sessionBuffer = Unpooled.Buffer();
                sessionBuffer.WriteLong(sessionId);
                sessionBuffer.WriteBytes(uselessBytes);

                Resources.Players.Add(new SessionContext
                {
                    PlayerId = sessionId,
                    EndPoint = packet.Sender
                });

                await ctx.WriteAsync(new DatagramPacket(sessionBuffer, packet.Sender));
            }
            else
            {
                Logger.Log($"Received {length} bytes: {BitConverter.ToString(content.Array.Take(length).ToArray()).Replace("-", "")}", null, ErrorLevel.Debug);

                var sessionId = content.ReadLong();
                content.ReadBytes(2);

                var sessionContext = Resources.Players.Get(sessionId);

                if (sessionContext != null)
                {
                    sessionContext.Active = true;

                    if (length != 10)
                    {
                        sessionContext.Process(content, ctx.Channel);
                    }
                    else
                    {
                        var sessionOkBuffer = Unpooled.Buffer();
                        sessionOkBuffer.WriteLong(sessionContext.PlayerId);
                        sessionOkBuffer.WriteByte(16);
                        sessionOkBuffer.WriteByte(0);
                        await ctx.WriteAsync(new DatagramPacket(sessionOkBuffer, sessionContext.EndPoint));
                    }
                }
                else
                {
                    Logger.Log("Player not logged in.", null, ErrorLevel.Debug);
                }
            }
        }
    }
}
