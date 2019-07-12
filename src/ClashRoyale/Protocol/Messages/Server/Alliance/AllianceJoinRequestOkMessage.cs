using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class AllianceJoinRequestOkMessage : PiranhaMessage
    {
        public AllianceJoinRequestOkMessage(Device device) : base(device)
        {
            Id = 24319;
        }
    }
}