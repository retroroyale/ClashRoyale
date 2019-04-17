using ClashRoyale.Files;
using ClashRoyale.Files.CsvLogic;
using Newtonsoft.Json;

namespace ClashRoyale.Logic.Home
{
    public class Arena
    {
        [JsonIgnore] public Home Home { get; set; }

        [JsonProperty("arena")] public int CurrentArena { get; set; }
        [JsonProperty("trophies")] public int Trophies { get; set; }

        public Arena()
        {
            CurrentArena = 1;
        }

        public async void AddTrophies(int trophies)
        {
            while (ArenaData(CurrentArena + 1).TrophyLimit <= Trophies + trophies)
            {
                CurrentArena++;
            }

            Trophies += trophies;

            if (Home.AllianceInfo.HasAlliance)
            {
                var alliance = await Resources.Alliances.GetAllianceAsync(Home.AllianceInfo.Id);

                var member = alliance?.GetMember(Home.Id);
                if (member == null) return;

                member.Score = Trophies;
                alliance.Save();
            }
        }

        public Arenas ArenaData(int arena) => Csv.Tables.Get(Csv.Files.Arenas).GetDataWithInstanceId<Arenas>(arena);
    }
}