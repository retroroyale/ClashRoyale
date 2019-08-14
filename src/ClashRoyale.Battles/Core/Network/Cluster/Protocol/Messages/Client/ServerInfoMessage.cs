using ClashRoyale.Utilities.Netty;

namespace ClashRoyale.Battles.Core.Network.Cluster.Protocol.Messages.Client
{
    public class ServerInfoMessage : ClusterMessage
    {
        public ServerInfoMessage()
        {
            Id = 12000;
        }

        public override void Encode()
        {
            Writer.WriteVInt(Resources.Sessions.Count);
            Writer.WriteVInt(Resources.Configuration.MaxSessions);
        }
    }
}