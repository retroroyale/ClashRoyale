using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;
using Newtonsoft.Json;

namespace ClashRoyale.Logic.Home.StreamEntry.Entries
{
    public class AllianceMailAvatarStreamEntry : AvatarStreamEntry
    {
        public AllianceMailAvatarStreamEntry()
        {
            StreamEntryType = 6;
        }

        [JsonProperty("msg")] public string Message { get; set; }
        [JsonProperty("title")] public string Title { get; set; }
        [JsonProperty("allianceId")] public long AllianceId { get; set; }
        [JsonProperty("allianceName")] public string AllianceName { get; set; }
        [JsonProperty("allianceBadge")] public int AllianceBadge { get; set; }

        public override void Encode(IByteBuffer packet)
        {
            base.Encode(packet);

            packet.WriteScString(Message);

            packet.WriteLong(1); // ??

            packet.WriteScString(Title);
            packet.WriteLong(AllianceId);
            packet.WriteScString(AllianceName);

            packet.WriteVInt(16);
            packet.WriteVInt(AllianceBadge);
        }
    }
}