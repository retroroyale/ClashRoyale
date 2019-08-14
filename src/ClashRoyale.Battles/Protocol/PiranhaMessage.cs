using System;
using System.Threading.Tasks;
using ClashRoyale.Battles.Logic.Session;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;
using DotNetty.Transport.Channels.Sockets;
using SharpRaven.Data;

namespace ClashRoyale.Battles.Protocol
{
    public class PiranhaMessage
    {
        public PiranhaMessage(SessionContext sessionContext)
        {
            SessionContext = sessionContext;
            Writer = Unpooled.Buffer(5);
        }

        public PiranhaMessage(SessionContext sessionContext, IByteBuffer buffer)
        {
            SessionContext = sessionContext;
            Reader = buffer;
        }

        public IByteBuffer Writer { get; set; }
        public IByteBuffer Reader { get; set; }
        public SessionContext SessionContext { get; set; }
        public int Id { get; set; }
        public int Length { get; set; }

        public virtual void Decrypt()
        {
            if (Length <= 0) return;

            var buffer = Reader.ReadBytes(Length);

            SessionContext.Rc4.Decrypt(ref buffer);

            Reader = buffer;
            Length = buffer.ReadableBytes;
        }

        public virtual void Encrypt()
        {
            if (Writer.ReadableBytes <= 0) return;

            var buffer = Writer;

            SessionContext.Rc4.Encrypt(ref buffer);

            Length = buffer.ReadableBytes;
        }

        public virtual void Decode()
        {
        }

        public virtual void Encode()
        {
        }

        public virtual void Process()
        {
        }

        /// <summary>
        ///     Writes this message to the clients channel
        /// </summary>
        /// <returns></returns>
        public async Task SendAsync()
        {
            try
            {
                Encode();
                Encrypt();

                var buffer = Unpooled.Buffer(15 + Length);
                buffer.WriteLong(SessionContext.Session.Id);
                buffer.WriteByte(SessionContext.GameMode);
                buffer.WriteByte(SessionContext.Index);

                buffer.WriteByte(0);
                buffer.WriteByte(1);

                var seq = SessionContext.Seq;
                if (SessionContext.Seq == byte.MaxValue) SessionContext.Seq = 0;
                else SessionContext.Seq++;

                buffer.WriteByte(seq);
                buffer.WriteVInt(Id);
                buffer.WriteVInt(Length);

                buffer.WriteBytes(Writer);

                await SessionContext.Channel.WriteAndFlushAsync(new DatagramPacket(buffer, SessionContext.EndPoint));

                Logger.Log($"[S] Message {Id} ({GetType().Name}) sent.", GetType(), ErrorLevel.Debug);
            }
            catch (Exception)
            {
                Logger.Log($"[S] Failed to send {Id}.", GetType(), ErrorLevel.Debug);
            }
        }
    }
}