using System;
using System.Collections.Generic;
using ClashRoyale.Battles.Protocol.Commands;

namespace ClashRoyale.Battles.Protocol
{
    public class LogicCommandManager
    {
        public static Dictionary<int, Type> Commands;

        static LogicCommandManager()
        {
            Commands = new Dictionary<int, Type>
            {
                {1, typeof(DoSpellCommand)}
            };
        }
    }
}