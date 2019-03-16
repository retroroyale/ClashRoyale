using System;
using System.Collections.Generic;
using ClashRoyale.Protocol.Commands.Client;
using ClashRoyale.Protocol.Commands.Server;

namespace ClashRoyale.Protocol
{
    public class LogicCommandManager
    {
        public static Dictionary<int, Type> Commands;

        static LogicCommandManager()
        {
            Commands = new Dictionary<int, Type>
            {
                {1, typeof(DoSpellCommand)},
                {500, typeof(LogicSwapSpellsCommand)},
                {501, typeof(LogicSelectDeckCommand)},
                {504, typeof(LogicFuseSpellsCommand)},
               // {509, typeof()}, // OpenFreeChest
               // {511, typeof()}, // OpenCrownChest
                {513, typeof(LogicFreeWorkerCommand)},
                {516, typeof(LogicBuyChestCommand)},
                {517, typeof(LogicBuyResourcesCommand)},
                {518, typeof(LogicBuySpellCommand)},
                {525, typeof(StartMatchmakeCommand)}
            };
        }
    }
}