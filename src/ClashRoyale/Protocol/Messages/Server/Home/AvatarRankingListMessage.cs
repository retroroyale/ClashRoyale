using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;
using ClashRoyale.Utilities.Utils;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class AvatarRankingListMessage : PiranhaMessage
    {
        public AvatarRankingListMessage(Device device) : base(device)
        {
            Id = 24403;
        }

        public override void Encode()
        {
            var players = Resources.Leaderboard.GlobalPlayerRanking;
            var count = players.Count;

            Writer.WriteVInt(count);

            for (var i = 0; i < count; i++)
            {
                var player = players[i];

                Writer.WriteVInt(player.Home.HighId);
                Writer.WriteVInt(player.Home.LowId);
                Writer.WriteScString(player.Home.Name);

                Writer.WriteVInt(i + 1);
                Writer.WriteVInt(player.Home.Arena.Trophies);
                Writer.WriteVInt(200);

                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);

                player.RankingEntry(Writer);
            }

            Writer.WriteInt(0);
            Writer.WriteInt(TimeUtils.GetSecondsUntilNextMonth);
        }
    }
}