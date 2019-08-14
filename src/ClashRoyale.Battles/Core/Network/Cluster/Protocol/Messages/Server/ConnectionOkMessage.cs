using DotNetty.Buffers;

namespace ClashRoyale.Battles.Core.Network.Cluster.Protocol.Messages.Server
{
    public class ConnectionOkMessage : ClusterMessage
    {
        public ConnectionOkMessage(IByteBuffer buffer) : base(buffer)
        {
            Id = 20104;
        }
    }
}
