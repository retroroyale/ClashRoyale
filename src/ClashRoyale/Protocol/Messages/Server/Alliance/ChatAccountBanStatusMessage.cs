using ClashRoyale.Extensions;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class ChatAccountBanStatusMessage : PiranhaMessage
    {
        public ChatAccountBanStatusMessage(Device device) : base(device)
        {
            Id = 20118;
        }

        public int Reason { get; set; }

        // Reason:

        public override void Encode()
        {
            Writer.WriteVInt(Reason);
        }
    }
}