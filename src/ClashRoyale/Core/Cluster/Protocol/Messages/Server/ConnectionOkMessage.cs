namespace ClashRoyale.Core.Cluster.Protocol.Messages.Server
{
    public class ConnectionOkMessage : ClusterMessage
    {
        public ConnectionOkMessage(Node server) : base(server)
        {
            Id = 20104;
        }
    }
}