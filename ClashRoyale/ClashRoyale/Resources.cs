using System;
using System.Threading.Tasks;
using ClashRoyale.Core;
using ClashRoyale.Core.Leaderboards;
using ClashRoyale.Core.Network;
using ClashRoyale.Database;
using ClashRoyale.Database.Cache;
using ClashRoyale.Extensions.Utils;
using ClashRoyale.Files;

namespace ClashRoyale
{
    public static class Resources
    {
        public static Logger Logger { get; set; }
        public static SentryReport Sentry { get; set; }
        public static Configuration Configuration { get; set; }
        public static PlayerDb PlayerDb { get; set; }
        public static AllianceDb AllianceDb { get; set; }
        public static Redis Redis { get; set; }
        public static Leaderboard Leaderboard { get; set; }

        public static NettyService Netty { get; set; }

        public static Fingerprint Fingerprint { get; set; }
        public static Csv Csv { get; set; }
        public static UpdateManager UpdateManager { get; set; }
        public static Battles Battles { get; set; }
        public static Players Players { get; set; }
        public static Alliances Alliances { get; set; }

        public static async void Initialize()
        {
            Logger = new Logger();
            Logger.Log(
                $"Starting [{DateTime.Now.ToLongTimeString()} - {ServerUtils.GetOSName()}]...",
                null);

            Configuration = new Configuration();
            Configuration.Initialize();

            Fingerprint = new Fingerprint();
            Sentry = new SentryReport();
            Csv = new Csv();

            UpdateManager = new UpdateManager();

            PlayerDb = new PlayerDb();
            AllianceDb = new AllianceDb();

            Logger.Log($"Successfully loaded MySql with {await PlayerDb.CountAsync()} player(s) & {await AllianceDb.CountAsync()} clan(s)", null);

            Redis = new Redis();

            Battles = new Battles();
            Players = new Players();
            Alliances = new Alliances();

            Leaderboard = new Leaderboard();

            Netty = new NettyService();

            await Task.Factory.StartNew(Netty.RunServerAsync);
        }
    }
}