using System;
using Newtonsoft.Json;

namespace ClashRoyale.Logic.Clan
{
    public class AllianceInfo
    {
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("highId")] public int HighId { get; set; }
        [JsonProperty("lowId")] public int LowId { get; set; }
        [JsonProperty("badge")] public int Badge { get; set; }
        [JsonProperty("role")] public int Role { get; set; }

        [JsonIgnore] public bool HasAlliance => Id > 0;

        [JsonIgnore]
        public long Id
        {
            get => ((long) HighId << 32) | (LowId & 0xFFFFFFFFL);
            set
            {
                HighId = Convert.ToInt32(value >> 32);
                LowId = (int) value;
            }
        }

        public void Reset()
        {
            Id = 0;
            Name = string.Empty;
            Badge = 0;
            Role = 0;
        }
    }
}