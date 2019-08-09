using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class AllianceRankingListMessage : PiranhaMessage
    {
        public AllianceRankingListMessage(Device device) : base(device)
        {
            Id = 24401;
        }

        public override void Encode()
        {
            var alliances = Resources.Leaderboard.GlobalAllianceRanking;
            var count = alliances.Count;

            Writer.WriteVInt(count);

            for (var i = 0; i < count; i++)
            {
                var alliance = alliances[i];

                Writer.WriteVInt(alliance.HighId);
                Writer.WriteVInt(alliance.LowId);
                Writer.WriteScString(alliance.Name);

                Writer.WriteVInt(i + 1);
                Writer.WriteVInt(alliance.Score);
                Writer.WriteVInt(200);

                Writer.WriteVInt(16);
                Writer.WriteVInt(alliance.Badge);

                Writer.WriteVInt(57);
                Writer.WriteVInt(6);
                Writer.WriteVInt(alliance.Members.Count);
            }
        }
    }
}