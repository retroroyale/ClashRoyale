using ClashRoyale.Extensions;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class AllianceStreamEntryRemovedMessage : PiranhaMessage
    {
        public AllianceStreamEntryRemovedMessage(Device device) : base(device)
        {
            Id = 24318;
        }

        public int EntryId { get; set; }

        public override void Encode()
        {
            Packet.WriteVInt(32);
            Packet.WriteVInt(0);

            Packet.WriteVInt(0);
            Packet.WriteVInt(0);

            Packet.WriteVInt(0);
            Packet.WriteVInt(0);

            Packet.WriteVInt(0);
            Packet.WriteVInt(EntryId);
        }
    }
}