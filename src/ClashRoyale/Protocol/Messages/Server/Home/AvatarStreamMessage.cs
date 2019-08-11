using System.Collections.Generic;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Home.StreamEntry;
using ClashRoyale.Utilities.Netty;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class AvatarStreamMessage : PiranhaMessage
    {
        public AvatarStreamMessage(Device device) : base(device)
        {
            Id = 24411;
        }

        public List<AvatarStreamEntry> Entries { get; set; }

        public override void Encode()
        {
            Writer.WriteVInt(Entries.Count);

            foreach (var entry in Entries) entry.Encode(Writer);
        }
    }
}