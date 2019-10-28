using System;
using System.Collections.Generic;
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

        /// <summary>
        ///     Add trophies and update arena and clan
        /// </summary>
        /// <param name="trophies"></param>
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

        /// <summary>
        ///     When a player losses trophies he might drop into a lower Arena and update it in the clan
        /// </summary>
        /// <param name="trophies"></param>
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

        /// <summary>
        ///     If the players trophies change we also have to update the member entry of the clan
        /// </summary>
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

        /// <summary>
        ///     Returns the next ArenaData if available
        /// </summary>
        /// <returns></returns>
        public Arenas GetNextArenaData()
        {
            return ArenaData(CurrentArena + 1);
        }

        /// <summary>
        ///     Returns the current ArenaData
        /// </summary>
        /// <returns></returns>
        public Arenas GetCurrentArenaData()
        {
            return ArenaData(CurrentArena);
        }

        /// <summary>
        ///     Returns the previous ArenaData
        /// </summary>
        /// <returns></returns>
        public Arenas GetOldArenaData()
        {
            return ArenaData(CurrentArena - 1);
        }

        /// <summary>
        /// Returns a list of arenas the player was in up to the current one for chests
        /// </summary>
        /// <returns></returns>
        public List<string> GetChestArenaNames()
        {
            var list = new List<string>();

            for (var i = 0; i <= CurrentArena; i++)
            {
                var data = ArenaData(i);
                if (data != null) list.Add(data.ChestArena);
            }

            return list;
        }
    }
}