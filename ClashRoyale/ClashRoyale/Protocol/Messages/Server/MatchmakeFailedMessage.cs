using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class MatchmakeFailedMessage : PiranhaMessage
    {
        public MatchmakeFailedMessage(Device device) : base(device)
        {
            Id = 24108;
        }

        public override void Encode()
        {
            Packet.WriteInt(0);
        }
    }
}