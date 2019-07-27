using System;
using ClashRoyale.Utilities.Utils;

namespace ClashRoyale.Battles
{
    public static class Resources
    {
        public static Logger Logger { get; set; }

        public static void Initialize()
        {
            Logger = new Logger();
            Logger.Log(
                $"Starting [{DateTime.Now.ToLongTimeString()} - {ServerUtils.GetOsName()}]...",
                null);
        }
    }
}
