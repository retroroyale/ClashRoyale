using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class AllianceStreamEntryRemovedMessage : PiranhaMessage
    {
        public AllianceStreamEntryRemovedMessage(Device device) : base(device)
        {
            Id = 24318;
        }

        public long EntryId { get; set; }

        public override void Encode()
        {
            Writer.WriteLong(EntryId);
        }
    }
}