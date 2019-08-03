using ClashRoyale.Battles.Core.Network.Cluster.Handlers;
using ClashRoyale.Battles.Core.Network.Cluster.Protocol.Messages.Client;
using ClashRoyale.Utilities.Crypto;
using DotNetty.Buffers;

namespace ClashRoyale.Battles.Core.Network.Cluster
{
    public class ClusterClient
    {
        public void Process(IByteBuffer buffer)
        {
            // TODO
        }

        public async void Login()
        {
            await new ConnectionCheckMessage().SendAsync();
        }

        #region Objects

        public Rc4Core Rc4 = new Rc4Core(Resources.Configuration.ClusterKey, Resources.Configuration.ClusterNonce);
        public ClusterPacketHandler Handler { get; set; }

        #endregion Objects
    }
}
