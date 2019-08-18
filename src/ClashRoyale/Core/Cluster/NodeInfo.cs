using System.Text;

namespace ClashRoyale.Core.Cluster
{
    public class NodeInfo
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public string Nonce { get; set; }
        public int BattlesRunning { get; set; }
        public int MaxBattles { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"IP: {Ip}");
            sb.AppendLine($"Port: {Port}");
            sb.AppendLine($"Nonce: {Nonce}");
            sb.AppendLine($"Active Battles: {BattlesRunning}");
            sb.AppendLine($"Max Battles: {MaxBattles}");

            return sb.ToString();
        }
    }
}