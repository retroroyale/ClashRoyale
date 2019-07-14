using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class AllianceInvitationSentOkMessage : PiranhaMessage
    {
        public AllianceInvitationSentOkMessage(Device device) : base(device)
        {
            Id = 24322;
        }
    }
}