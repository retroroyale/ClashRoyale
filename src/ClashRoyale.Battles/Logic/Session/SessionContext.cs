using System;
using System.Linq;
using System.Net;
using ClashRoyale.Battles.Protocol;
using ClashRoyale.Utilities.Crypto;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using SharpRaven.Data;

namespace ClashRoyale.Battles.Logic.Session
{
    public class SessionContext
    {
        public Rc4Core Rc4 = new Rc4Core("fhsd6f86f67rt8fw78fw789we78r9789wer6re", "nonce");

        public Session Session { get; set; }
        public EndPoint EndPoint { get; set; }
        public IChannel Channel { get; set; }

        private DateTime _lastMessage = DateTime.UtcNow;
        private DateTime _lastCommands = DateTime.UtcNow;

        public byte Seq = 1;

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

        public bool BattleActive
        {
            get => DateTime.UtcNow.Subtract(_lastCommands).TotalSeconds < 10;
            set
            {
                if (value)
                {
                    _lastCommands = DateTime.UtcNow;
                }
            }
        }

        public async void Process(IByteBuffer reader, IChannel channel)
        {
            Channel = channel;

            var ackCount = reader.ReadByte();

            for (var i = 0; i < ackCount; i++)
            {
                var ack = reader.ReadByte();

                var buffer = Unpooled.Buffer();
                buffer.WriteLong(Session.Id);
                buffer.WriteBytes(new byte[2]);
                buffer.WriteByte(1); 
                buffer.WriteByte(ack); 
                await Channel.WriteAndFlushAsync(new DatagramPacket(buffer, EndPoint));
            }

            if (ackCount > 0)
                return;

            var chunkCount = reader.ReadVInt();

            for (var i = 0; i < chunkCount; i++)
            {
                var chunkSeq = reader.ReadByte();
                var chunkId = reader.ReadVInt();
                var chunkLength = reader.ReadVInt();

                if (!LogicMessageFactory.Messages.ContainsKey(chunkId))
                {
                    Logger.Log($"Message ID: {chunkId}, S: {chunkSeq}, L: {chunkLength} is not known.", GetType(),
                        ErrorLevel.Debug);
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

                     Logger.Log($"Message {chunkId} ({message.GetType().Name}) handled.", GetType(),
                         ErrorLevel.Debug);
                 }
                 catch (Exception exception)
                 {
                     Logger.Log($"Failed to process {chunkId}: " + exception, GetType(), ErrorLevel.Error);
                 }

                var buffer = Unpooled.Buffer();
                buffer.WriteLong(Session.Id);
                buffer.WriteBytes(new byte[2]);
                buffer.WriteByte(1); 
                buffer.WriteByte(chunkSeq); 
                await Channel.WriteAndFlushAsync(new DatagramPacket(buffer, EndPoint));
            }
            
            var readable = reader.ReadableBytes;
            if(readable > 0)
                Logger.Log($"{BitConverter.ToString(reader.ReadBytes(readable).Array.Take(readable).ToArray()).Replace("-", "")}", null, ErrorLevel.Debug);
        }
    }
}