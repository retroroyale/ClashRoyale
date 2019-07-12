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

        public override void Encode()
        {
            Writer.WriteVInt(Reason);
        }
    }
}