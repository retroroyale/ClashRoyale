using ClashRoyale.Extensions;
using DotNetty.Buffers;
using Newtonsoft.Json;

namespace ClashRoyale.Logic.Clan.StreamEntry.Entries
{
    public class ChallengeStreamEntry : AllianceStreamEntry
    {
        public ChallengeStreamEntry()
        {
            StreamEntryType = 10;
        }

        [JsonProperty("msg")] public string Message { get; set; }
        [JsonProperty("sender_score")] public int SenderScore { get; set; }

        public override void Encode(IByteBuffer packet)
        {
            base.Encode(packet);

            packet.WriteScString(Message);

            packet.WriteBoolean(false);

            packet.WriteVInt(SenderScore);

            packet.WriteBoolean(false);
            packet.WriteBoolean(false);

            packet.WriteVInt(1);

            packet.WriteBoolean(false);

            packet.WriteBoolean(false);
            packet.WriteBoolean(false);
        }

        public override void SetSender(Player player)
        {
            base.SetSender(player);

            SenderScore = player.Home.Trophies;
        }
    }
}