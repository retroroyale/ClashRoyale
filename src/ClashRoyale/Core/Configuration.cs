using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ClashRoyale.Core
{
    public class Configuration
    {
        [JsonIgnore] public static JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ObjectCreationHandling = ObjectCreationHandling.Reuse,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.None
        };

        [JsonProperty("battle_servers")] public List<string> BattleServers = new List<string>();

        // Make sure to edit these on prod
        [JsonProperty("cluster_encryption_key")]
        public string ClusterKey = "15uvmi8qnyuj9tm53ipaavvytltm582yatecyjzb";

        [JsonProperty("cluster_encryption_nonce")]
        public string ClusterNonce = "nonce";

        [JsonProperty("cluster_server_port")] public int ClusterServerPort = 9876;

        [JsonProperty("encryption_key")] public string EncryptionKey = "fhsd6f86f67rt8fw78fw789we78r9789wer6re";

        [JsonProperty("mysql_database")] public string MySqlDatabase = "rrdb";
        [JsonProperty("mysql_password")] public string MySqlPassword = "";
        [JsonProperty("mysql_server")] public string MySqlServer = "127.0.0.1";
        [JsonProperty("mysql_user")] public string MySqlUserId = "root";

        [JsonProperty("patch_url")] public string PatchUrl = "";
        [JsonProperty("redis_password")] public string RedisPassword = "";
        [JsonProperty("redis_server")] public string RedisServer = "127.0.0.1";
        [JsonProperty("sentry_api")] public string SentryApiUrl = "";

        [JsonProperty("server_port")] public int ServerPort = 9339;
        [JsonProperty("update_url")] public string UpdateUrl = "https://github.com/retroroyale/ClashRoyale";
        [JsonProperty("use_content_patch")] public bool UseContentPatch;

        [JsonProperty("use_udp")] public bool UseUdp;

        /// <summary>
        ///     Loads the configuration
        /// </summary>
        public void Initialize()
        {
            if (File.Exists("config.json"))
                try
                {
                    var config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("config.json"));

                    EncryptionKey = config.EncryptionKey;
                    SentryApiUrl = config.SentryApiUrl;

                    MySqlUserId = config.MySqlUserId;
                    MySqlServer = config.MySqlServer;
                    MySqlPassword = config.MySqlPassword;
                    MySqlDatabase = config.MySqlDatabase;
                    RedisPassword = config.RedisPassword;
                    RedisServer = config.RedisServer;

                    PatchUrl = config.PatchUrl;
                    UseContentPatch = config.UseContentPatch;

                    ServerPort = config.ServerPort;
                    UpdateUrl = config.UpdateUrl;

                    UseUdp = config.UseUdp;
                    BattleServers = config.BattleServers;
                    ClusterServerPort = config.ClusterServerPort;

                    ClusterKey = config.ClusterKey;
                    ClusterNonce = config.ClusterNonce;
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
                    Save();

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

        public void Save()
        {
            File.WriteAllText("config.json", JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}