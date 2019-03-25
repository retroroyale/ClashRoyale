using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClashRoyale.Database;
using ClashRoyale.Extensions;
using ClashRoyale.Logic.Clan.StreamEntry;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;
using Newtonsoft.Json;
using SharpRaven.Data;

namespace ClashRoyale.Logic.Clan
{
    public class Alliance
    {
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
        [JsonProperty("highId")] public int HighId { get; set; }
        [JsonProperty("lowId")] public int LowId { get; set; }
        [JsonProperty("badge")] public int Badge { get; set; }
        [JsonProperty("region")] public int Region { get; set; }
        [JsonProperty("type")] public int Type { get; set; }
        [JsonProperty("requiredScore")] public int RequiredScore { get; set; }
        [JsonProperty("stream")] public List<AllianceStreamEntry> Stream = new List<AllianceStreamEntry>(40);
        [JsonProperty("members")] public List<AllianceMember> Members = new List<AllianceMember>(50);

        public Alliance(long id)
        {
            Id = id;
            Name = "RetroRoyale";
        }

        public void AllianceRankingEntry(IByteBuffer packet)
        {
            // TODO
        }

        public void AllianceFullEntry(IByteBuffer packet)
        {
            AllianceHeaderEntry(packet);

            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);

            packet.WriteVInt(91);
            packet.WriteVInt(0); // Donations per week
            packet.WriteVInt(0);

            packet.WriteVInt(1);
            packet.WriteVInt(0);
            packet.WriteVInt(57);

            packet.WriteVInt(94);
            packet.WriteVInt(0);

            packet.WriteScString(Description);
        }

        public void AllianceHeaderEntry(IByteBuffer packet)
        {
            packet.WriteLong(Id);
            packet.WriteScString(Name);

            packet.WriteVInt(16);
            packet.WriteVInt(Badge);

            packet.WriteVInt(Type);
            packet.WriteVInt(Members.Count);

            packet.WriteVInt(Score);
            packet.WriteVInt(RequiredScore);
        }

        [JsonIgnore]
        public int Score => Members.Sum(m => m.Score) / 2;

        [JsonIgnore]
        public int Online => Members.Count(m => m.IsOnline);

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

        public AllianceInfo GetAllianceInfo(long userId) =>
            new AllianceInfo
            {
                Id = Id,
                Name = Name,
                Badge = Badge,
                Role = GetRole(userId)
            };

        public void Remove(long id)
        {
            var index = Members.FindIndex(x => x.Id == id);

            if (index > -1)
            {
                Members.RemoveAt(index);
            }
        }

        public async void AddEntry(AllianceStreamEntry entry)
        {
            while (Stream.Count >= 40)
                Stream.RemoveAt(0);

            Stream.Add(entry);

            foreach (var member in Members.Where(m => m.IsOnline))
            {
                var player = await Resources.Players.GetPlayer(member.Id, true);

                if (player != null)
                {
                    await new AllianceStreamEntryMessage(player.Device)
                    {
                        Entry = entry
                    }.Send();
                }
            }
        }

        public int GetRole(long id)
        {
            var index = Members.FindIndex(x => x.Id == id);

            return index > -1 ? Members[index].Role : 1;
        }

        public async void UpdateOnlineCount()
        {
            var count = Online;

            foreach (var member in Members.Where(m => m.IsOnline))
            {
                var player = await Resources.Players.GetPlayer(member.Id, true);

                if (player != null)
                {
                    await new AllianceOnlineStatusUpdatedMessage(player.Device)
                    {
                        Count = count
                    }.Send();
                }
            }
        }

        public async void Save()
        {
            var st = new Stopwatch();
            st.Start();

            await Redis.Cache(this);
            await AllianceDb.Save(this);

            st.Stop();
            Logger.Log($"Alliance {Id} saved in {st.ElapsedMilliseconds}ms.", GetType(), ErrorLevel.Debug);
        }

        public enum Role
        {
            Member = 1,
            Leader = 2,
            Elder = 3,
            CoLeader = 4
        }
    }
}