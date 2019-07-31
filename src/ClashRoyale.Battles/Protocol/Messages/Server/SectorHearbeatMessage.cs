using System.Collections.Generic;
using ClashRoyale.Battles.Logic.Session;
using ClashRoyale.Utilities.Netty;

namespace ClashRoyale.Battles.Protocol.Messages.Server
{
    public class SectorHearbeatMessage : PiranhaMessage
    {
        public SectorHearbeatMessage(SessionContext ctx) : base(ctx)
        {
            Id = 21902;
        }

        public int Turn { get; set; }
        public Queue<byte[]> Commands { get; set; }

        public override void Encode()
        {
            Writer.WriteVInt(Turn);
            Writer.WriteVInt(0);

            Writer.WriteVInt(Commands.Count);

            for (var i = 0; i < Commands.Count; i++) Writer.WriteBytes(Commands.Dequeue());
        }
    }
}