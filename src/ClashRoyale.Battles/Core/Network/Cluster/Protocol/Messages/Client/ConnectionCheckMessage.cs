using System;
using System.Collections.Generic;
using System.Text;

namespace ClashRoyale.Battles.Core.Network.Cluster.Protocol.Messages.Client
{
    public class ConnectionCheckMessage : ClusterMessage
    {
        public ConnectionCheckMessage()
        {
            Id = 10101;
        }

        public override void Encode()
        {
            // TODO
        }
    }
}
