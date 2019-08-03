using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;
using SharpRaven.Data;

namespace ClashRoyale.Core.Cluster.Protocol.Messages.Client
{
    public class ConnectionCheckMessage : ClusterMessage
    {
        public ConnectionCheckMessage(Server server, IByteBuffer buffer) : base(server, buffer)
        {
            Id = 10101;
        }

        public string Nonce { get; set; }
        public int Port { get; set; }
        public int MaxBattles { get; set; }
        public bool CryptoFailed { get; set; }

        public override void Decode()
        {
            CryptoFailed = Reader.ReadBoolean();

            if (CryptoFailed) return;

            Nonce = Reader.ReadScString();
            Port = Reader.ReadVInt();
            MaxBattles = Reader.ReadVInt();
        }

        public override void Process()
        {
            if (CryptoFailed)
            {
                Logger.Log($"Failed to decrypt packet of battle server {Server.GetIp()}.", GetType(), ErrorLevel.Warning);
                return;
            }

            Logger.Log($"Battle server with nonce \"{Nonce}\" connected.", GetType());

            // TODO SEND OK
        }
    }
}
