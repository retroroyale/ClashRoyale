using System;
using System.Threading.Tasks;
using ClashRoyale.Battles.Core;
using ClashRoyale.Battles.Core.Network;
using ClashRoyale.Battles.Core.Network.Cluster;
using ClashRoyale.Utilities.Utils;

namespace ClashRoyale.Battles
{
    public static class Resources
    {
        public static Logger Logger { get; set; }
        public static NettyService Netty { get; set; }
        public static NettyClient NettyClient { get; set; }
        public static Sessions Sessions { get; set; }
        public static Configuration Configuration { get; set; }

        public static async void Initialize()
        {
            Logger = new Logger();
            Logger.Log(
                $"Starting [{DateTime.Now.ToLongTimeString()} - {ServerUtils.GetOsName()}]...",
                null);

            Configuration = new Configuration();
            Configuration.Initialize();

            Sessions = new Sessions();
            Netty = new NettyService();
            NettyClient = new NettyClient();

            await Task.Run(Netty.RunServerAsync);
            await Task.Run(NettyClient.RunClientAsync);
        }
    }
}
