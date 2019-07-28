using ClashRoyale.Battles.Logic.Session;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;

namespace ClashRoyale.Battles.Protocol
{
    public class LogicCommand
    {
        public LogicCommand(SessionContext sessionContext)
        {
            SessionContext = sessionContext;
            Data = Unpooled.Buffer();
        }

        public LogicCommand(SessionContext sessionContext, IByteBuffer buffer)
        {
            SessionContext = sessionContext;
            Buffer = buffer;
            Data = Unpooled.Buffer();
        }

        public IByteBuffer Data { get; set; }
        public SessionContext SessionContext { get; set; }

        public int Type { get; set; }
        public int Tick { get; set; }
        public IByteBuffer Buffer { get; set; }

        public virtual void Decode()
        {
            Tick = Buffer.ReadVInt();
            Buffer.ReadVInt();
        }

        public virtual void Encode()
        {
        }

        public virtual void Process()
        {
        }
    }
}