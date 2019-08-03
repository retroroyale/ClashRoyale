namespace ClashRoyale.Battles.Core.Network.Cluster.Protocol.Messages.Client
{
    public class BattleFinishedMessage : ClusterMessage
    {
        public BattleFinishedMessage()
        {
            Id = 11000;
        }

        public long SessionId { get; set; }

        public override void Encode()
        {
            Writer.WriteLong(SessionId);
        }
    }
}
