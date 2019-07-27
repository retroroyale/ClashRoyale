using System.Collections.Generic;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Clan;
using ClashRoyale.Utilities.Netty;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class AllianceListMessage : PiranhaMessage
    {
        public AllianceListMessage(Device device) : base(device)
        {
            Id = 24310;
        }

        public List<Alliance> Alliances { get; set; }
        public string Query { get; set; }

        public override void Encode()
        {
            Writer.WriteScString(Query);
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