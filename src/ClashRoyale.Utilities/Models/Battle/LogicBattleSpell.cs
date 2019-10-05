using Newtonsoft.Json;

namespace ClashRoyale.Utilities.Models.Battle
{
    public class LogicBattleSpell
    {
        [JsonProperty("d")] public int Id { get; set; }
        [JsonProperty("l")] public int Level { get; set; }
    }
}