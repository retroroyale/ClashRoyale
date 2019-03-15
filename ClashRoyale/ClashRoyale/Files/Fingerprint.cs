using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ClashRoyale.Files
{
    public class Fingerprint
    {
        public Fingerprint()
        {
            try
            {
                if (File.Exists("GameAssets/fingerprint.json"))
                {
                    Json = File.ReadAllText("GameAssets/fingerprint.json");
                    var json = JObject.Parse(Json);
                    Sha = json["sha"].ToObject<string>();
                    Version = json["version"].ToObject<string>().Split('.').Select(int.Parse) as int[];

                    Logger.Log($"Fingerprint v.{json["version"].ToObject<string>()} has been loaded into memory.",
                        GetType());
                }
                else
                {
                    Console.WriteLine("The Fingerprint cannot be loaded, the file does not exist.");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load the Fingerprint.");
            }
        }

        public string Json { get; set; }

        public string Sha { get; set; }

        public int[] Version { get; set; }

        public int GetMajorVersion => Version?[0] ?? 3;
        public int GetBuildVersion => Version?[1] ?? 377;
        public int GetContentVersion => Version?[2] ?? 3;

        public string GetVersion => GetMajorVersion + "." + GetBuildVersion + "." + GetContentVersion;
    }
}