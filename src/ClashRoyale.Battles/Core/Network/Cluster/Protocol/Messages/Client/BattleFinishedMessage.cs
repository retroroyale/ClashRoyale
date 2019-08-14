namespace ClashRoyale.Battles.Core.Network.Cluster.Protocol.Messages.Client
{
    public class BattleFinishedMessage : ClusterMessage
    {
        public BattleFinishedMessage()
        {
            Id = 11000;
        }

        public long SessionId { get; set; }
        public byte Gamemode { get; set; }
        public byte Index { get; set; }

        public override void Encode()
        {
            Writer.WriteLong(SessionId);
            Writer.WriteByte(Gamemode);
            Writer.WriteByte(Index);
        }
    }
}
