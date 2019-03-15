using ClashRoyale.Extensions;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class BattleEventData : PiranhaMessage
    {
        public BattleEventData(Device device) : base(device)
        {
            Id = 22952;
        }

        public int Tick { get; set; }
        public int EventId { get; set; }
        public int HighId { get; set; }
        public int LowId { get; set; }
        public int Value { get; set; }

        public override void Encode()
        {
            Packet.WriteVInt(EventId);
            Packet.WriteVInt(HighId);
            Packet.WriteVInt(LowId);
            Packet.WriteVInt(1);
            Packet.WriteVInt(Tick);
            Packet.WriteVInt(0);
            Packet.WriteVInt(1);
            Packet.WriteVInt(Value);
        }
    }
}