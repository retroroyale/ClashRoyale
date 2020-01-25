using System;
using System.Threading.Tasks;
using ClashRoyale.Core;
using ClashRoyale.Core.Cluster;
using ClashRoyale.Core.Leaderboards;
using ClashRoyale.Core.Network;
using ClashRoyale.Database;
using ClashRoyale.Database.Cache;
using ClashRoyale.Files;
using ClashRoyale.Logic.Home.Decks;
using ClashRoyale.Utilities.Utils;

namespace ClashRoyale
{
    public static class Resources
    {
        public static Logger Logger { get; set; }
        public static SentryReport Sentry { get; set; }
        public static Configuration Configuration { get; set; }
        public static PlayerDb PlayerDb { get; set; }
        public static AllianceDb AllianceDb { get; set; }
        public static ObjectCache ObjectCache { get; set; }
        public static Leaderboard Leaderboard { get; set; }

        public static NettyService Netty { get; set; }
        public static NodeManager NodeManager { get; set; }

        public static Fingerprint Fingerprint { get; set; }
        public static Csv Csv { get; set; }
        public static UpdateManager UpdateManager { get; set; }
        public static Battles Battles { get; set; }
        public static DuoBattles DuoBattles { get; set; }
        public static Players Players { get; set; }
        public static Alliances Alliances { get; set; }

        public static DateTime StartTime { get; set; }

        public static async void Initialize()
        {
            Logger = new Logger();
            Logger.Log(
                $"Starting [{DateTime.Now.ToLongTimeString()} - {ServerUtils.GetOsName()}]...",
                null);

            Configuration = new Configuration();
            Configuration.Initialize();

            NodeManager = new NodeManager();

            Fingerprint = new Fingerprint();
            Sentry = new SentryReport();
            Csv = new Csv();
            Cards.Initialize();

            UpdateManager = new UpdateManager();

            PlayerDb = new PlayerDb();
            AllianceDb = new AllianceDb();

            Logger.Log(
                $"Successfully loaded MySql with {await PlayerDb.CountAsync()} player(s) & {await AllianceDb.CountAsync()} clan(s)",
                null);

            ObjectCache = new ObjectCache();

            Battles = new Battles();
            DuoBattles = new DuoBattles();
            Players = new Players();
            Alliances = new Alliances();

            Leaderboard = new Leaderboard();

            StartTime = DateTime.UtcNow;

            Netty = new NettyService();

            await Task.Run(Netty.RunServerAsync);
        }
    }
}