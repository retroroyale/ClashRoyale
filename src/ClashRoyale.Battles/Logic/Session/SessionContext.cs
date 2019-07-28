using System;
using System.Linq;
using System.Net;
using ClashRoyale.Battles.Protocol;
using ClashRoyale.Utilities.Crypto;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using SharpRaven.Data;

namespace ClashRoyale.Battles.Logic.Session
{
    public class SessionContext
    {
        public Rc4Core Rc4 = new Rc4Core("fhsd6f86f67rt8fw78fw789we78r9789wer6re", "scroll");

        public long PlayerId { get; set; }
        public EndPoint EndPoint { get; set; }
        public IChannel Channel { get; set; }

        private DateTime _lastMessage = DateTime.UtcNow;

        public bool Active
        {
            get => DateTime.UtcNow.Subtract(_lastMessage).TotalSeconds < 10;
            set
            {
                if (value)
                {
                    _lastMessage = DateTime.UtcNow;
                }
            }
        }

        public void Process(IByteBuffer reader, IChannel channel)
        {
            Channel = channel;

            var ackCount = reader.ReadVInt();
            var chunkCount = reader.ReadVInt();

            Logger.Log($"Ack Count: {ackCount}", null, ErrorLevel.Debug);
            Logger.Log($"Chunk Count: {chunkCount}", null, ErrorLevel.Debug);

            for (var i = 0; i < chunkCount; i++)
            {
                var chunkSeq = reader.ReadVInt();
                var chunkId = reader.ReadVInt();
                var chunkLength = reader.ReadVInt();

                Logger.Log($"Chunk Seq:    {chunkSeq}", null, ErrorLevel.Debug);

                if (!LogicMessageFactory.Messages.ContainsKey(chunkId))
                {
                    Logger.Log($"Message ID: {chunkId}, S: {chunkSeq}, L: {chunkLength} is not known.", GetType(),
                        ErrorLevel.Warning);
                    return;
                }

                if (!(Activator.CreateInstance(LogicMessageFactory.Messages[chunkId], this, reader) is PiranhaMessage
                    message)) continue;

                try
                {
                    message.Id = chunkId;
                    message.Length = chunkLength;

                    message.Decrypt();
                    message.Decode();
                    message.Process();

                    Logger.Log($"[C] Message {chunkId} ({message.GetType().Name}) handled.", GetType(),
                        ErrorLevel.Debug);
                }
                catch (Exception exception)
                {
                    Logger.Log($"Failed to process {chunkId}: " + exception, GetType(), ErrorLevel.Error);
                }
            }

            var readable = reader.ReadableBytes;
            if(readable > 0)
                Logger.Log($"{BitConverter.ToString(reader.ReadBytes(readable).Array.Take(readable).ToArray()).Replace("-", "")}", null, ErrorLevel.Debug);
        }
    }
}