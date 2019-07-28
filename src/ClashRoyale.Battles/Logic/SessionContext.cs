using System;
using System.Linq;
using System.Net;
using ClashRoyale.Utilities.Crypto;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;
using SharpRaven.Data;

namespace ClashRoyale.Battles.Logic
{
    public class SessionContext
    {
        public Rc4Core Rc4 = new Rc4Core("fhsd6f86f67rt8fw78fw789we78r9789wer6re", "scroll");
        public long PlayerId { get; set; }
        public EndPoint EndPoint { get; set; }

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

        public void Process(IByteBuffer reader)
        {
            var ackCount = reader.ReadVInt();
            var chunkCount = reader.ReadVInt();

            Logger.Log($"Player {PlayerId} sent us some spicy data", null, ErrorLevel.Debug);
            Logger.Log($"Ack Count: {ackCount}", null, ErrorLevel.Debug);
            Logger.Log($"Chunk Count: {chunkCount}", null, ErrorLevel.Debug);

            for (var i = 0; i < chunkCount; i++)
            {
                Logger.Log($"Chunk Sequence: {reader.ReadVInt()}", null, ErrorLevel.Debug);
                Logger.Log($"Chunk Id: {reader.ReadVInt()}", null, ErrorLevel.Debug);
                Logger.Log($"Chunk Length: {reader.ReadVInt()}", null, ErrorLevel.Debug);
            }

            var readable = reader.ReadableBytes;
            Logger.Log($"{BitConverter.ToString(reader.ReadBytes(readable).Array.Take(readable).ToArray()).Replace("-", "")}", null, ErrorLevel.Debug);
        }
    }
}