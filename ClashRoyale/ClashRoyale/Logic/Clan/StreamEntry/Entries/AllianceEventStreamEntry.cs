using ClashRoyale.Extensions;
using DotNetty.Buffers;
using Newtonsoft.Json;

namespace ClashRoyale.Logic.Clan.StreamEntry.Entries
{
    public class AllianceEventStreamEntry : AllianceStreamEntry
    {
        public AllianceEventStreamEntry()
        {
            StreamEntryType = 4;
        }

        [JsonProperty("eventType")] public Type EventType { get; set; }
        [JsonProperty("targetHighId")] public int TargetHighId { get; set; }
        [JsonProperty("targetLowId")] public int TargetLowId { get; set; }
        [JsonProperty("targetName")] public string TargetName { get; set; }

        public override void Encode(IByteBuffer packet)
        {
            base.Encode(packet);

            packet.WriteVInt((int)EventType);

            packet.WriteVInt(TargetHighId);
            packet.WriteVInt(TargetLowId);

            packet.WriteScString(TargetName);
        }

        public enum Type
        {
            Kick = 1,
            Accepted = 2,
            Join = 3,
            Leave = 4,
            Promote = 5,
            Demote = 6
        }
    }
}