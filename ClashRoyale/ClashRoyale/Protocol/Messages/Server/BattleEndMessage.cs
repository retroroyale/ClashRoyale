using ClashRoyale.Extensions;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class BattleEndMessage : PiranhaMessage
    {
        public BattleEndMessage(Device device) : base(device)
        {
            Id = 20225;
        }

        public override void Encode()
        {
            Writer.WriteVInt(1);
            Writer.WriteVInt(31); // Trophies (Own)

            Writer.WriteVInt(0);
            Writer.WriteVInt(35); // Trophies (Opponent)

            Writer.WriteVInt(0);
            Writer.WriteVInt(63);

            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(3);
            Writer.WriteVInt(0);
            Writer.WriteVInt(19);
            Writer.WriteVInt(225);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(4);
            Writer.WriteVInt(47);
            Writer.WriteVInt(1260);
            Writer.WriteVInt(1293);
            Writer.WriteVInt(11);
            Writer.WriteVInt(1260);

            // Treasure Chest
            Writer.WriteVInt(58);
            Writer.WriteVInt(205);

            Writer.WriteVInt(21);
            Writer.WriteVInt(1);
        }
    }
}