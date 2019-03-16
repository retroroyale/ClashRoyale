using ClashRoyale.Extensions;
using ClashRoyale.Extensions.Utils;
using ClashRoyale.Logic;
using DotNetty.Buffers;

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
            var count = 0;
            var buffer = Unpooled.Buffer();

            foreach (var player in Resources.Leaderboard.GlobalPlayers)
            {
                if (player == null) continue;

                buffer.WriteVInt(player.Home.HighId);
                buffer.WriteVInt(player.Home.LowId);
                buffer.WriteScString(player.Home.Name);

                buffer.WriteVInt(count + 1);
                buffer.WriteVInt(3800);
                buffer.WriteVInt(200);

                buffer.WriteVInt(0);
                buffer.WriteVInt(0);
                buffer.WriteVInt(0);

                player.RankingEntry(buffer);

                count++;
            }

            Packet.WriteVInt(count);
            Packet.WriteBytes(buffer);

            Packet.WriteInt(0);
            Packet.WriteInt(TimeUtils.GetSecondsUntilNextMonth);
        }
    }
}