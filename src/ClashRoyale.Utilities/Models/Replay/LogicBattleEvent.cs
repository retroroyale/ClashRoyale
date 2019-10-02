using System.Collections.Generic;
using Newtonsoft.Json;

namespace ClashRoyale.Utilities.Models.Replay
{
    public class LogicBattleEvent
    {
        [JsonProperty("type")] public int Type { get; set; }
        [JsonProperty("id_hi")] public int HighId { get; set; }
        [JsonProperty("id_lo")] public int LowId { get; set; }
        [JsonProperty("params")] public List<int> Params = new List<int>();
        [JsonProperty("ticks")] public List<int> Ticks = new List<int>();
    }
}