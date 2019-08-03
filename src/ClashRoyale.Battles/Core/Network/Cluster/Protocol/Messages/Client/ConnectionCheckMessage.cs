using ClashRoyale.Utilities.Netty;

namespace ClashRoyale.Battles.Core.Network.Cluster.Protocol.Messages.Client
{
    public class ConnectionCheckMessage : ClusterMessage
    {
        public ConnectionCheckMessage()
        {
            Id = 10101;
        }

        public override void Encode()
        {
            Writer.WriteBoolean(false);

            Writer.WriteScString(Resources.Configuration.BattleNonce);
            Writer.WriteVInt(Resources.Configuration.ServerPort);
            Writer.WriteVInt(Resources.Configuration.MaxBattles);
        }
    }
}
