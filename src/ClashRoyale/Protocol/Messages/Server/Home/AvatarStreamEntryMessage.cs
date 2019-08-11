using ClashRoyale.Logic;
using ClashRoyale.Logic.Home.StreamEntry;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class AvatarStreamEntryMessage : PiranhaMessage
    {
        public AvatarStreamEntryMessage(Device device) : base(device)
        {
            Id = 24412;
        }

        public AvatarStreamEntry Entry { get; set; }

        public override void Encode()
        {
            Entry.Encode(Writer);
        }
    }
}