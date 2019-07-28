using System;
using System.Linq;
using ClashRoyale.Utilities.Netty;
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

            Logger.Log($"Received {length} bytes", null, ErrorLevel.Debug);

            if (length == 1400)
            {
                var playerId = content.ReadLong();
                var uselessBytes = content.ReadBytes(2);

                var sessionBuffer = Unpooled.Buffer();
                sessionBuffer.WriteLong(playerId);
                sessionBuffer.WriteBytes(uselessBytes);

                Resources.Players.Add(new SessionContext
                {
                    PlayerId = playerId
                });

                await ctx.WriteAsync(new DatagramPacket(sessionBuffer, packet.Sender));

                Logger.Log("OK!", null, ErrorLevel.Debug);
            }
            else
            {
                var playerId = content.ReadLong();
                content.ReadBytes(2);
                var ackCount = content.ReadVInt();
                var chunkCount = content.ReadVInt();

                var sessionContext = Resources.Players.Get(playerId);

                if (sessionContext != null)
                {
                    sessionContext.Active = true;

                    Logger.Log($"Player {playerId} sent us some spicy data", null, ErrorLevel.Debug);
                    Logger.Log($"Ack Count: {ackCount}", null, ErrorLevel.Debug);
                    Logger.Log($"Chunk Count: {chunkCount}", null, ErrorLevel.Debug);

                    for (var i = 0; i < chunkCount; i++)
                    {
                        Logger.Log($"Chunk Sequence: {content.ReadVInt()}", null, ErrorLevel.Debug);
                        Logger.Log($"Chunk Id: {content.ReadVInt()}", null, ErrorLevel.Debug);
                        Logger.Log($"Chunk Length: {content.ReadVInt()}", null, ErrorLevel.Debug);
                    }

                    var readable = content.ReadableBytes;
                    Logger.Log($"{BitConverter.ToString(content.ReadBytes(readable).Array.Take(readable).ToArray()).Replace("-", "")}", null, ErrorLevel.Debug);
                }
                else
                {
                    Logger.Log("Player not logged in.", null, ErrorLevel.Debug);
                }
            }
        }
    }
}
