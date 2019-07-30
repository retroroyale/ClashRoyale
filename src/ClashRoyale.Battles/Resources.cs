using System;
using System.Threading.Tasks;
using ClashRoyale.Battles.Core;
using ClashRoyale.Battles.Core.Network;
using ClashRoyale.Utilities.Utils;

namespace ClashRoyale.Battles
{
    public static class Resources
    {
        public static Logger Logger { get; set; }
        public static NettyService Netty { get; set; }
        public static Sessions Sessions { get; set; }

        public static async void Initialize()
        {
            Logger = new Logger();
            Logger.Log(
                $"Starting [{DateTime.Now.ToLongTimeString()} - {ServerUtils.GetOsName()}]...",
                null);

            Sessions = new Sessions();
            Netty = new NettyService();

            await Task.Run(Netty.RunServerAsync);
        }
    }
}
