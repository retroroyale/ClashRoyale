namespace ClashRoyale.Core.Cluster.Protocol.Messages.Server
{
    public class ConnectionOkMessage : ClusterMessage
    {
        public ConnectionOkMessage(Cluster.Server server) : base(server)
        {
            Id = 20104;
        }
    }
}
