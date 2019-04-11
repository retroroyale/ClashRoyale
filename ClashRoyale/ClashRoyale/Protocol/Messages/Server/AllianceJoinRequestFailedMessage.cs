using ClashRoyale.Extensions;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class AllianceJoinRequestFailedMessage : PiranhaMessage
    {
        public AllianceJoinRequestFailedMessage(Device device) : base(device)
        {
            Id = 24320;
        }

        public int Reason { get; set; }

        public override void Encode()
        {
            Writer.WriteVInt(Reason);
        }
    }
}