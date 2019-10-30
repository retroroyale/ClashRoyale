using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class BattleEventMessage : PiranhaMessage
    {
        public BattleEventMessage(Device device) : base(device)
        {
            Id = 22952;
        }

        public int Tick { get; set; }
        public int Type { get; set; }
        public int HighId { get; set; }
        public int LowId { get; set; }
        public int Value1 { get; set; }
        public int Value2 { get; set; }

        public int Unknown { get; set; }
        public int Unknown2 { get; set; }
        public int Unknown3 { get; set; }
        public int HandIndex { get; set; }

        public override void Encode()
        {
            Writer.WriteVInt(Type);
            Writer.WriteVInt(HighId);
            Writer.WriteVInt(LowId);
            Writer.WriteVInt(1);
            Writer.WriteVInt(Tick);
            Writer.WriteVInt(Unknown3);
            Writer.WriteVInt(Value1);
            Writer.WriteVInt(Value2);

            if (Type == 6)
            {
                Writer.WriteVInt(Unknown);
                Writer.WriteVInt(HandIndex);
                Writer.WriteVInt(Unknown2);
            }
        }
    }
}