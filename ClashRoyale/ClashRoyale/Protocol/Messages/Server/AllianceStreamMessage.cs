using System.Collections.Generic;
using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Clan.StreamEntry;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class AllianceStreamMessage : PiranhaMessage
    {
        public AllianceStreamMessage(Device device) : base(device)
        {
            Id = 24311;
        }

        public List<AllianceStreamEntry> Entries { get; set; }

        public override void Encode()
        {
            Packet.WriteVInt(Entries.Count);

            foreach (var entry in Entries) entry.Encode(Packet);
        }
    }
}