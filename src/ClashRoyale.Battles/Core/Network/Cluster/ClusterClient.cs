using ClashRoyale.Battles.Core.Network.Cluster.Handlers;
using ClashRoyale.Utilities.Crypto;
using DotNetty.Buffers;

namespace ClashRoyale.Battles.Core.Network.Cluster
{
    public class ClusterClient
    {
        public static void Process(IByteBuffer buffer)
        {
            // TODO
        }

        #region Objects

        public static Rc4Core Rc4 = new Rc4Core("fhsd6f86f67rt8fw78fw789we78r9789wer6re", "nonce");
        public static ClusterPacketHandler Handler { get; set; }

        #endregion Objects
    }
}
