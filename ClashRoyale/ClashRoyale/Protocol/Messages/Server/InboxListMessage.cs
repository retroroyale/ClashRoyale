using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class InboxListMessage : PiranhaMessage
    {
        public InboxListMessage(Device device) : base(device)
        {
            Id = 24445;
        }

        public override void Encode()
        {
            Writer.WriteInt(0);
        }
    }
}