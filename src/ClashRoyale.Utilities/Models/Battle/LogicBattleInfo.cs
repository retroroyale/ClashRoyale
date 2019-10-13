using System.Collections.Generic;
using Newtonsoft.Json;

namespace ClashRoyale.Utilities.Models.Battle
{
    public class LogicBattleInfo
    {
        [JsonProperty("gmt")] public int Gmt = 1;
        [JsonProperty("gamemode")] public int Gamemode = 72000006;
        [JsonProperty("arena")] public int Arena = 54000012;
        [JsonProperty("location")] public int Location = 15000013;

        [JsonProperty("deck0")] public List<LogicBattleSpell> Deck0 = new List<LogicBattleSpell>();
        [JsonProperty("deck1")] public List<LogicBattleSpell> Deck1 = new List<LogicBattleSpell>();
        [JsonProperty("avatar0")] public LogicBattleAvatar Avatar0 = new LogicBattleAvatar();
        [JsonProperty("avatar1")] public LogicBattleAvatar Avatar1 = new LogicBattleAvatar();
    }
}
