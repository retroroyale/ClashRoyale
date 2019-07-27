using System;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;
using Newtonsoft.Json;

namespace ClashRoyale.Logic.Home.StreamEntry
{
    public class AvatarStreamEntry
    {
        [JsonProperty("creation")] public DateTime CreationDateTime = DateTime.UtcNow;
        [JsonProperty("id")] public int Id = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        [JsonProperty("type")] public int StreamEntryType { get; set; }
        [JsonProperty("highId")] public int SenderHighId { get; set; }
        [JsonProperty("lowId")] public int SenderLowId { get; set; }
        [JsonProperty("sender_name")] public string SenderName { get; set; }
        [JsonProperty("sender_role")] public int SenderRole { get; set; }
        [JsonProperty("removed")] public bool IsRemoved { get; set; }
        [JsonProperty("new")] public bool IsNew { get; set; }

        [JsonIgnore] public int AgeSeconds => (int) (DateTime.UtcNow - CreationDateTime).TotalSeconds;

        [JsonIgnore]
        public long SenderId
        {
            get => ((long) SenderHighId << 32) | (SenderLowId & 0xFFFFFFFFL);
            set
            {
                SenderHighId = Convert.ToInt32(value >> 32);
                SenderLowId = (int) value;
            }
        }

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

            packet.WriteVInt(AgeSeconds);

            packet.WriteBoolean(IsRemoved);
            packet.WriteBoolean(IsNew);
        }

        public virtual void SetSender(Player player)
        {
            SenderName = player.Home.Name;
            SenderId = player.Home.Id;
            SenderRole = player.Home.AllianceInfo.Role;
        }
    }
}