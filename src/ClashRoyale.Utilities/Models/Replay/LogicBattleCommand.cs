using Newtonsoft.Json;

namespace ClashRoyale.Utilities.Models.Replay
{
    public class LogicBattleCommand
    {
        [JsonProperty("t")] public int Tick { get; set; }
        [JsonProperty("t2")] public int Tick2 { get; set; }
        [JsonProperty("idHi")] public int HighId { get; set; }
        [JsonProperty("idLo")] public int LowId { get; set; }
        [JsonProperty("idx")] public int Idx { get; set; }
        [JsonProperty("gid")] public int GlobalId { get; set; }
        [JsonProperty("px")] public int PositionX { get; set; }
        [JsonProperty("py")] public int PositionY { get; set; }
        [JsonProperty("sid")] public int Sid { get; set; }
    }
}