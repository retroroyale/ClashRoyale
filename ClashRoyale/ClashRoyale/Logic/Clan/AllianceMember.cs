using System;
using DotNetty.Buffers;
using Newtonsoft.Json;

namespace ClashRoyale.Logic.Clan
{
    public class AllianceMember
    {
        [JsonProperty("highId")] public int HighId { get; set; }
        [JsonProperty("lowId")] public int LowId { get; set; }
        [JsonProperty("role")] public int Role { get; set; }
        [JsonProperty("score")] public int Score { get; set; }
        [JsonProperty("donations")] public int Donations { get; set; }
        [JsonProperty("donationsReceived")] public int DonationsReceived { get; set; }

        public AllianceMember(long id, Alliance.Role role, int score)
        {
            Id = id;
            Role = (int)role;
            Score = score;
        }

        public void AllianceMemberEntry(IByteBuffer packet)
        {
            // TODO
        }

        [JsonIgnore]
        public long Id
        {
            get => ((long)HighId << 32) | (LowId & 0xFFFFFFFFL);
            set
            {
                HighId = Convert.ToInt32(value >> 32);
                LowId = (int)value;
            }
        }

        [JsonIgnore]
        public bool IsOnline => Resources.Players.ContainsKey(Id);
    }
}
