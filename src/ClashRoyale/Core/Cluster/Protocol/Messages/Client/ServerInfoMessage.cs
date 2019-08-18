using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;

namespace ClashRoyale.Core.Cluster.Protocol.Messages.Client
{
    public class ServerInfoMessage : ClusterMessage
    {
        public ServerInfoMessage(Node server, IByteBuffer buffer) : base(server, buffer)
        {
            Id = 12000;
        }

        public int BattlesRunning { get; set; }
        public int MaxBattles { get; set; }

        public override void Decode()
        {
            BattlesRunning = Reader.ReadVInt();
            MaxBattles = Reader.ReadVInt();
        }

        public override void Process()
        {
            var info = Server.NodeInfo;
            info.MaxBattles = MaxBattles;
            info.BattlesRunning = BattlesRunning;
        }
    }
}