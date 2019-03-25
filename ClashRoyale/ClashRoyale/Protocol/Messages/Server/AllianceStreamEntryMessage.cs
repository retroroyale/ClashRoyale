using ClashRoyale.Logic;
using ClashRoyale.Logic.Clan.StreamEntry;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class AllianceStreamEntryMessage : PiranhaMessage
    {
        public AllianceStreamEntryMessage(Device device) : base(device)
        {
            Id = 24312;
        }

        public AllianceStreamEntry Entry { get; set; }

        public override void Encode()
        {
            Entry.Encode(Packet);
        }
    }
}