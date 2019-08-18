using System;
using System.Collections.Generic;
using ClashRoyale.Core.Cluster.Protocol.Messages.Client;

namespace ClashRoyale.Core.Cluster.Protocol
{
    public class ClusterMessageFactory
    {
        public static Dictionary<int, Type> Messages;

        static ClusterMessageFactory()
        {
            Messages = new Dictionary<int, Type>
            {
                {10101, typeof(ConnectionCheckMessage)},
                {11000, typeof(BattleFinishedMessage)},
                {12000, typeof(ServerInfoMessage)}
            };
        }
    }
}