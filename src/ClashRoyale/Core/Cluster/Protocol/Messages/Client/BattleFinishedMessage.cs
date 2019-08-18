using DotNetty.Buffers;

namespace ClashRoyale.Core.Cluster.Protocol.Messages.Client
{
    public class BattleFinishedMessage : ClusterMessage
    {
        public BattleFinishedMessage(Node server, IByteBuffer buffer) : base(server, buffer)
        {
            Id = 11000;
        }

        public long SessionId { get; set; }
        public byte Gamemode { get; set; }
        public byte Index { get; set; }

        public override void Decode()
        {
            SessionId = Reader.ReadLong();
            Gamemode = Reader.ReadByte();
            Index = Reader.ReadByte();
        }

        public override void Process()
        {
            if (Gamemode == 0)
            {
                var battle = Resources.Battles.Get(SessionId);
                battle?.Stop(Index);
            }

            // TODO: DUO
        }
    }
}