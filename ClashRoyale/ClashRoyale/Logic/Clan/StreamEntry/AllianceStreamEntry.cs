using System;
using ClashRoyale.Extensions;
using DotNetty.Buffers;
using Newtonsoft.Json;

namespace ClashRoyale.Logic.Clan.StreamEntry
{
    public class AllianceStreamEntry
    {
        [JsonProperty("type")] public int StreamEntryType { get; set; }
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("highId")] public int SenderHighId { get; set; }
        [JsonProperty("lowId")] public int SenderLowId { get; set; }
        [JsonProperty("sender_name")] public string SenderName { get; set; }
        [JsonProperty("sender_role")] public int SenderRole { get; set; }
        [JsonProperty("removed")] public bool IsRemoved { get; set; }
        [JsonProperty("creation")] public DateTime CreationDateTime { get; set; }

        [JsonIgnore]
        public int AgeSeconds => (int)(DateTime.UtcNow - CreationDateTime).TotalSeconds;

        public virtual void Encode(IByteBuffer packet)
        {
            packet.WriteVInt(StreamEntryType);
            packet.WriteVInt(0);

            packet.WriteVInt(Id);

            packet.WriteVInt(SenderHighId);
            packet.WriteVInt(SenderLowId);
            packet.WriteVInt(SenderHighId);
            packet.WriteVInt(SenderLowId);

            packet.WriteScString(SenderName);
            packet.WriteVInt(0); // Level
            packet.WriteVInt(SenderRole);

            packet.WriteVInt(AgeSeconds);
            packet.WriteBoolean(IsRemoved);
        }

        public virtual void SetSender(Player player)
        {
            SenderName = player.Home.Name;
            SenderId = player.Home.Id;
            SenderRole = player.Home.AllianceInfo.Role;
        }

        [JsonIgnore]
        public long SenderId
        {
            get => ((long)SenderHighId << 32) | (SenderLowId & 0xFFFFFFFFL);
            set
            {
                SenderHighId = Convert.ToInt32(value >> 32);
                SenderLowId = (int)value;
            }
        }
    }
}
