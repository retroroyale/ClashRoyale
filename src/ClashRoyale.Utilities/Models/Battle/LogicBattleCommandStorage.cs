using Newtonsoft.Json;

namespace ClashRoyale.Utilities.Models.Battle
{
    public class LogicBattleCommandStorage
    {
        [JsonProperty("ct")] public int CommandType { get; set; }
        [JsonProperty("c")] public LogicBattleCommand Command = new LogicBattleCommand();
    }
}