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
            Writer.WriteVInt(EventId);
            Writer.WriteVInt(HighId);
            Writer.WriteVInt(LowId);
            Writer.WriteVInt(1);
            Writer.WriteVInt(Tick);
            Writer.WriteVInt(0);
            Writer.WriteVInt(1);
            Writer.WriteVInt(Value);
        }
    }
}