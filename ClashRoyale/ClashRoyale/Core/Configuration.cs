using System;
using System.IO;
using Newtonsoft.Json;

namespace ClashRoyale.Core
{
    public class Configuration
    {
        [JsonIgnore] public const string Version = "0.1";

        [JsonProperty("encryption_key")] public string EncryptionKey = "fhsd6f86f67rt8fw78fw789we78r9789wer6re";

        [JsonProperty("mysql_database")] public string MySqlDatabase = "rrdb";

        [JsonProperty("mysql_password")] public string MySqlPassword = "";

        [JsonProperty("mysql_server")] public string MySqlServer = "127.0.0.1";

        [JsonProperty("mysql_user")] public string MySqlUserId = "root";

        [JsonProperty("patch_url")] public string PatchUrl = "";

        [JsonProperty("redis_password")] public string RedisPassword = "";

        [JsonProperty("redis_server")] public string RedisServer = "127.0.0.1";

        [JsonProperty("server_port")] public int ServerPort = 9339;

        [JsonProperty("update_url")] public string UpdateUrl = "https://github.com/retroroyale/ClashRoyale";

        public void Initialize()
        {
            if (File.Exists("config.json"))
                try
                {
                    var config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("config.json"));

                    UpdateUrl = config.UpdateUrl;
                    PatchUrl = config.PatchUrl;
                    MySqlUserId = config.MySqlUserId;
                    MySqlServer = config.MySqlServer;
                    MySqlPassword = config.MySqlPassword;
                    MySqlDatabase = config.MySqlDatabase;
                    RedisPassword = config.RedisPassword;
                    RedisServer = config.RedisServer;
                    EncryptionKey = config.EncryptionKey;
                    ServerPort = config.ServerPort;
                }
                catch (Exception)
                {
                    Console.WriteLine("Couldn't load configuration.");
                    Console.ReadKey(true);
                    Environment.Exit(0);
                }
            else
                try
                {
                    File.WriteAllText("config.json", JsonConvert.SerializeObject(this, Formatting.Indented));

                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("Server configuration has been created. Restart the server now.");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                catch (Exception)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Couldn't create config file.");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
        }
    }
}