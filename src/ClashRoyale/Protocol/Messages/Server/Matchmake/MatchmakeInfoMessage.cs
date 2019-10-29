using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class MatchmakeInfoMessage : PiranhaMessage
    {
        public MatchmakeInfoMessage(Device device) : base(device)
        {
            Id = 24107;
        }

        public int EstimatedDuration { get; set; }

        public override void Encode()
        {
            Writer.WriteInt(EstimatedDuration);
        }
    }
}