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

        // Reason:
        // 1 = Alliance closed
        // 2 = Wait before sending another request
        // 3 = Not enough trophies to join
        // 4 = Banned from alliance / Old request was rejected
        // 5 = can't join

        public override void Encode()
        {
            Writer.WriteVInt(Reason);
        }
    }
}