using System.Collections.Generic;
using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Clan;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class JoinableAllianceListMessage : PiranhaMessage
    {
        public JoinableAllianceListMessage(Device device) : base(device)
        {
            Id = 24304;
        }

        public List<Alliance> Alliances { get; set; }

        public override void Encode()
        {
            Writer.WriteVInt(Alliances.Count);

            foreach (var alliance in Alliances)
            {
                alliance.AllianceHeaderEntry(Writer);

                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);

                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);

                Writer.WriteVInt(1);
                Writer.WriteVInt(3);
                Writer.WriteVInt(57);

                Writer.WriteVInt(6);
                Writer.WriteVInt(0);
            }
        }
    }
}