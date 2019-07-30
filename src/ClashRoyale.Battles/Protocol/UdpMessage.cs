using ClashRoyale.Battles.Logic.Session;
using DotNetty.Buffers;

namespace ClashRoyale.Battles.Protocol
{
    public class UdpMessage
    {
        public UdpMessage(SessionContext sessionContext)
        {
            SessionContext = sessionContext;
        }

        public UdpMessage(SessionContext sessionContext, int id, IByteBuffer reader)
        {
            SessionContext = sessionContext;
            Id = id;
            Reader = reader;
        }

        public SessionContext SessionContext { get; set; }
        public int Id { get; set; }
        public PiranhaMessage PiranhaMessage { get; set; }
        public IByteBuffer Reader { get; set; }

        public void Encode()
        {
            // TODO
        }

        public void Decode()
        {
            // TODO
        }
    }
}