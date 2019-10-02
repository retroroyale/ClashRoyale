using Newtonsoft.Json;

namespace ClashRoyale.Utilities.Models.Replay
{
    public class LogicBattleAvatar
    {
        [JsonProperty("accountId.hi")] public int HighId { get; set; }
        [JsonProperty("accountId.lo")] public int LowId { get; set; }
        [JsonProperty("expLevel")] public int ExpLevel { get; set; }
        [JsonProperty("expPoints")] public int ExpPoints { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("clan_name")] public string ClanName { get; set; }
        [JsonProperty("arena")] public int Arena { get; set; }
        [JsonProperty("badge")] public int ClanBadge { get; set; }
        [JsonProperty("clan_id_hi")] public int ClanHighId { get; set; }
        [JsonProperty("clan_id_lo")] public int ClanLowId { get; set; }
    }
}