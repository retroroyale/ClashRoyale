using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
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
                    Files = new List<Asset>();

                    var json = JObject.Parse(Json);
                    {
                        Sha = json["sha"].ToObject<string>();
                        Version = json["version"].ToObject<string>().Split('.').Select(int.Parse) as int[];

                        foreach (var file in json["files"]) Files.Add(file.ToObject<Asset>());

                        Logger.Log($"Fingerprint v.{json["version"].ToObject<string>()} loaded.",
                            GetType());
                    }
                }
                else
                {
                    Console.WriteLine("The Fingerprint cannot be loaded, the file does not exist.");
                    Program.Exit();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load the Fingerprint.");
                Program.Exit();
            }
        }

        public string Json { get; set; }
        public string Sha { get; set; }
        public int[] Version { get; set; }
        public List<Asset> Files { get; set; }

        public int GetMajorVersion => Version?[0] ?? 3;
        public int GetBuildVersion => Version?[1] ?? 377;
        public int GetContentVersion => Version?[2] ?? 3;

        public string GetVersion => GetMajorVersion + "." + GetBuildVersion + "." + GetContentVersion;
    }

    public class Asset
    {
        [JsonProperty("file")] public string File { get; set; }
        [JsonProperty("sha")] public string Sha { get; set; }
    }
}