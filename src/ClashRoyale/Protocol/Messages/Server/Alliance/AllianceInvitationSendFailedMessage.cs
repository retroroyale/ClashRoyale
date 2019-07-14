using ClashRoyale.Extensions;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class AllianceInvitationSendFailedMessage : PiranhaMessage
    {
        public AllianceInvitationSendFailedMessage(Device device) : base(device)
        {
            Id = 24321;
        }

        public int Reason { get; set; }

        // Reason:
        // 2 = only Leaders and Co-Leaders can invite
        // 4 = player already joined clan
        // 5 = already invited
        // 6 = inbox full

        public override void Encode()
        {
            Writer.WriteVInt(Reason);
        }
    }
}