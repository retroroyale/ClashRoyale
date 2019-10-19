using System;
using ClashRoyale.Files;
using ClashRoyale.Files.CsvLogic;
using Newtonsoft.Json;

namespace ClashRoyale.Logic.Home
{
    public class Arena
    {
        public Arena()
        {
            CurrentArena = 1;
        }

        [JsonIgnore] public Home Home { get; set; }

        [JsonProperty("arena")] public int CurrentArena { get; set; }
        [JsonProperty("trophies")] public int Trophies { get; set; }

        public void AddTrophies(int trophies)
        {
            while (true)
            {
                var data = GetNextArenaData();
                if (data == null) break;

                if (data.TrophyLimit <= Trophies + trophies)
                    CurrentArena = data.Arena;
                else
                    break;
            }

            Trophies += trophies;

            UpdateClanTrophies();
        }

        public void RemoveTrophies(int trophies)
        {
            while (true)
            {
                var data = GetCurrentArenaData();
                if (data == null) break;

                if (data.TrophyLimit < Trophies - trophies)
                {
                    var oldArena = GetOldArenaData();
                    if (oldArena != null)
                        CurrentArena = oldArena.Arena;
                    else
                        break;
                }
                else
                {
                    break;
                }
            }

            Trophies += trophies;

            UpdateClanTrophies();
        }

        public async void UpdateClanTrophies()
        {
            if (!Home.AllianceInfo.HasAlliance) return;

            var alliance = await Resources.Alliances.GetAllianceAsync(Home.AllianceInfo.Id);

            var member = alliance?.GetMember(Home.Id);
            if (member == null) return;

            member.Score = Trophies;
            alliance.Save();
        }

        public Arenas ArenaData(int arena)
        {
            try
            {
                var table = Csv.Tables.Get(Csv.Files.Arenas);
                var index = table.Data.FindIndex(x => ((Arenas) x).Arena == arena);

                return index == -1 ? null : table.Data[index] as Arenas;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Arenas GetNextArenaData()
        {
            return ArenaData(CurrentArena + 1);
        }

        public Arenas GetCurrentArenaData()
        {
            return ArenaData(CurrentArena);
        }

        public Arenas GetOldArenaData()
        {
            return ArenaData(CurrentArena - 1);
        }
    }
}