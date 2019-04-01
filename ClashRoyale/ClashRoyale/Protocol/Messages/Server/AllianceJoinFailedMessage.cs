using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class AllianceJoinFailedMessage : PiranhaMessage
    {
        public AllianceJoinFailedMessage(Device device) : base(device)
        {
            Id = 24302;
        }

        public override void Encode()
        {
            Writer.WriteByte(0);
        }
    }
}