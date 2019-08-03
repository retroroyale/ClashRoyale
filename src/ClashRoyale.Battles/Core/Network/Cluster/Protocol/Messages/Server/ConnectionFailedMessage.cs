using DotNetty.Buffers;

namespace ClashRoyale.Battles.Core.Network.Cluster.Protocol.Messages.Server
{
    public class ConnectionFailedMessage : ClusterMessage
    {
        public ConnectionFailedMessage(IByteBuffer buffer) : base(buffer)
        {
            Id = 20103;
        }

        public override void Process()
        {
            // TODO
        }
    }
}
