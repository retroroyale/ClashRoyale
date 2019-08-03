namespace ClashRoyale.Core.Cluster
{
    public class ServerInfo
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public string Nonce { get; set; }
        public int BattlesRunning { get; set; }
    }
}