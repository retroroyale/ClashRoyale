using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class AvatarLocalRankingListMessage : PiranhaMessage
    {
        public AvatarLocalRankingListMessage(Device device) : base(device)
        {
            Id = 24404;
        }

        public override void Encode()
        {
            var players = Resources.Leaderboard.LocalPlayerRanking[Device.Player.Home.PreferredDeviceLanguage];
            var count = players.Count;

            Writer.WriteVInt(count);

            for (var i = 0; i < count; i++)
            {
                var player = players[i];

                Writer.WriteVInt(player.Home.HighId);
                Writer.WriteVInt(player.Home.LowId);
                Writer.WriteScString(player.Home.Name);

                Writer.WriteVInt(count + 1);
                Writer.WriteVInt(player.Home.Arena.Trophies);
                Writer.WriteVInt(200);

                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);

                player.RankingEntry(Writer);
            }
        }
    }
}