using ClashRoyale.Core;
using ClashRoyale.Core.Network;
using ClashRoyale.Database;
using ClashRoyale.Database.Cache;
using ClashRoyale.Files;

namespace ClashRoyale
{
    public class Resources
    {
        public static Logger Logger { get; set; }
        public static SentryReport Sentry { get; set; }
        public static Configuration Configuration { get; set; }
        public static PlayerDb PlayerDb { get; set; }
        public static Redis Redis { get; set; }

        public static NettyService Netty { get; set; }

        public static Fingerprint Fingerprint { get; set; }
        public static Csv Csv { get; set; }
        public static Battles Battles { get; set; }
        public static Players Players { get; set; }

        public static async void Initialize()
        {
            Logger = new Logger();
            Logger.Log("Starting...", null);

            Configuration = new Configuration();
            Configuration.Initialize();

            PlayerDb = new PlayerDb();
            Redis = new Redis();

            Fingerprint = new Fingerprint();
            Sentry = new SentryReport();
            Csv = new Csv();

            Battles = new Battles();
            Players = new Players();

            Netty = new NettyService();
            await Netty.RunServerAsync();
        }
    }
}