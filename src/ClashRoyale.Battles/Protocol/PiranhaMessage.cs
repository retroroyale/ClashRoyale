using ClashRoyale.Battles.Logic;
using DotNetty.Buffers;

namespace ClashRoyale.Battles.Protocol
{
    public class PiranhaMessage
    {
        public PiranhaMessage(SessionContext sessionContext)
        {
            SessionContext = sessionContext;
            Writer = Unpooled.Buffer(7);
        }

        public PiranhaMessage(SessionContext sessionContext, IByteBuffer buffer)
        {
            SessionContext = sessionContext;
            Reader = buffer;
        }

        public IByteBuffer Writer { get; set; }
        public IByteBuffer Reader { get; set; }
        public SessionContext SessionContext { get; set; }
        public ushort Id { get; set; }
        public int Length { get; set; }
        public ushort Version { get; set; }

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
    }
}