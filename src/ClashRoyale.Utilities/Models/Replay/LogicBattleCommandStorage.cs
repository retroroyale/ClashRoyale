using Newtonsoft.Json;

namespace ClashRoyale.Utilities.Models.Replay
{
    public class LogicBattleCommandStorage
    {
        [JsonProperty("ct")] public int Count { get; set; }
        [JsonProperty("c")] public LogicBattleCommand Command = new LogicBattleCommand();
    }
}