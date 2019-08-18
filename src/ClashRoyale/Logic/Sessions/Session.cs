using System;
using Newtonsoft.Json;

namespace ClashRoyale.Logic.Sessions
{
    public class Session
    {
        [JsonProperty("location")] public Location Location = new Location();
        [JsonIgnore] public DateTime SessionStart = DateTime.UtcNow;
        [JsonProperty("ip")] public string Ip { get; set; }
        [JsonProperty("duration")] public int Duration { get; set; }
        [JsonProperty("startDate")] public string StartDate { get; set; }
        [JsonProperty("deviceCode")] public string DeviceCode { get; set; }
        [JsonProperty("gameVersion")] public string GameVersion { get; set; }
        [JsonProperty("sessionId")] public string SessionId { get; set; }
    }
}