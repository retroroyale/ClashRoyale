using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class AvatarStreamEntryRemovedMessage : PiranhaMessage
    {
        public AvatarStreamEntryRemovedMessage(Device device) : base(device)
        {
            Id = 24418;
        }

        public long StreamEntryId { get; set; }

        public override void Encode()
        {
            Writer.WriteLong(StreamEntryId);
        }
    }
}