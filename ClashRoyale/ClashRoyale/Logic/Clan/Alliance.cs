using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClashRoyale.Database;
using ClashRoyale.Logic.Clan.StreamEntry;
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
            // TODO
        }

        public void AllianceHeaderEntry(IByteBuffer packet)
        {
            // TODO
        }

        [JsonIgnore]
        public int Score => Members.Sum(m => m.Score) / 2;

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

        public void AddEntry(AllianceStreamEntry entry)
        {
            while (Stream.Count >= 40)
                Stream.RemoveAt(0);

            Stream.Add(entry);
        }

        public bool IsMember(long id)
        {
            return Members.FindIndex(x => x.Id == id) != -1;
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