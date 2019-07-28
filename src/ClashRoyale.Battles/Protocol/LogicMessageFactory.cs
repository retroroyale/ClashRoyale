using System;
using System.Collections.Generic;
using ClashRoyale.Battles.Protocol.Client;

namespace ClashRoyale.Battles.Protocol
{
    public class LogicMessageFactory
    {
        public static Dictionary<int, Type> Messages;

        static LogicMessageFactory()
        {
            Messages = new Dictionary<int, Type>
            {
                {10108, typeof(UdpCheckConnectionMessage)}
            };
        }
    }
}
