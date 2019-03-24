using System;
using ClashRoyale.Extensions;
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
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("donations")] public int Donations { get; set; }
        [JsonProperty("donationsReceived")] public int DonationsReceived { get; set; }

        public AllianceMember(Player player, Alliance.Role role)
        {
            Id = player.Home.Id;
            Role = (int)role;
            Score = player.Home.Trophies;
            Name = player.Home.Name;
        }

        public void AllianceMemberEntry(IByteBuffer packet)
        {
            packet.WriteLong(Id); // ID
            packet.WriteScString(Name); // Name

            // Arena
            packet.WriteVInt(54);
            packet.WriteVInt(11); 

            packet.WriteVInt(Role); // Role
            packet.WriteVInt(0); // Level
            packet.WriteVInt(Score); // Trophies

            packet.WriteVInt(0); // Donated
            packet.WriteVInt(0); // Donations Received

            packet.WriteVInt(0); // Current Rank
            packet.WriteVInt(0); // Previus Rank

            packet.WriteVInt(0); // Chest Crowns
            packet.WriteVInt(65039); // Chest ??
            packet.WriteVInt(63);
            packet.WriteVInt(63);
            packet.WriteVInt(31);
            packet.WriteVInt(7);

            packet.WriteLong(Id); 
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
