using System.Collections.Generic;
using ClashRoyale.Extensions;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class SectorHearbeatMessage : PiranhaMessage
    {
        public SectorHearbeatMessage(Device device) : base(device)
        {
            Id = 21902;
        }

        public int Turn { get; set; }
        public Queue<byte[]> Commands { get; set; }

        public override void Encode()
        {
            Packet.WriteVInt(Turn);
            Packet.WriteVInt(0);

            Packet.WriteVInt(Commands.Count);

            for (var i = 0; i < Commands.Count; i++)
                Packet.WriteBuffer(Commands.Dequeue());
        }
    }
}