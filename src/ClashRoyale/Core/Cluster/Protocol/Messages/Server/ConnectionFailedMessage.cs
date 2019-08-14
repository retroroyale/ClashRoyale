using ClashRoyale.Utilities.Netty;

namespace ClashRoyale.Core.Cluster.Protocol.Messages.Server
{
    public class ConnectionFailedMessage : ClusterMessage
    {
        public ConnectionFailedMessage(Cluster.Server server) : base(server)
        {
            Id = 20103;
        }

        public int Error { get; set; }

        // Codes:
        // 1 = Crypto Error

        public override void Encode()
        {
            Writer.WriteVInt(Error);
        }
    }
}
