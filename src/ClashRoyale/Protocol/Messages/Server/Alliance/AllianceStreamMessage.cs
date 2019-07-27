using System.Collections.Generic;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Clan.StreamEntry;
using ClashRoyale.Utilities.Netty;

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
            Writer.WriteVInt(Entries.Count);

            foreach (var entry in Entries) entry.Encode(Writer);
        }
    }
}