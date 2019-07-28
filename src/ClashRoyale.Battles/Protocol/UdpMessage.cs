using ClashRoyale.Battles.Logic;

namespace ClashRoyale.Battles.Protocol
{
    public class UdpMessage
    {
        public UdpMessage(SessionContext sessionContext)
        {
            SessionContext = sessionContext;
        }

        public UdpMessage(SessionContext sessionContext, int id)
        {
            SessionContext = sessionContext;
            Id = id;
        }

        public SessionContext SessionContext { get; set; }
        public int Id { get; set; }
        public PiranhaMessage PiranhaMessage { get; set; }

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
