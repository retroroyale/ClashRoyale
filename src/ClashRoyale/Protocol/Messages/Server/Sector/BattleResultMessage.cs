using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class BattleResultMessage : PiranhaMessage
    {
        public BattleResultMessage(Device device) : base(device)
        {
            Id = 20225;
        }

        public int TrophyReward { get; set; }
        public int OpponentTrophyReward { get; set; }

        public override void Encode()
        {
            Writer.WriteVInt(1);
            Writer.WriteVInt(TrophyReward); // Trophies (Own)

            Writer.WriteVInt(0);
            Writer.WriteVInt(OpponentTrophyReward); // Trophies (Opponent)

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