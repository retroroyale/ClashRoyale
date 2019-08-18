using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SharpRaven.Data;

namespace ClashRoyale.Logic.Sessions
{
    public class Location
    {
        [JsonProperty("country")] public string CountryName { get; set; }
        [JsonProperty("countryCode")] public string CountryCode { get; set; }
        [JsonProperty("city")] public string City { get; set; }

        public static async Task<Location> GetByIpAsync(string ip)
        {
            try
            {
                //TODO: should check any local ip
                if (ip == "127.0.0.1" || ip.StartsWith("192")) return null;

                using (var client = new HttpClient())
                {
                    var json = await client.GetStringAsync("http://ip-api.com/json/" + ip);
                    return JsonConvert.DeserializeObject<Location>(json);
                }
            }
            catch (Exception)
            {
                Logger.Log($"Couldn't track location of {ip}", null, ErrorLevel.Error);
                return null;
            }
        }
    }
}