using System.Collections.Generic;
using System.Linq;

namespace ClashRoyale.Core.Cluster
{
    public class ServerManager : Dictionary<string, ServerInfo>
    {
        private readonly object _syncLock = new object();

        public new void Add(string host, ServerInfo info)
        {
            lock (_syncLock)
            {
                if (host.Contains(':'))
                {
                    base.Add(host, info);
                }
            }
        }

        public new void Remove(string host)
        {
            lock (_syncLock)
            {
                if (!host.Contains(':')) return;
                if (ContainsKey(host))
                {
                    base.Remove(host);
                }
            }
        }

        public ServerInfo GetServer()
        {
            lock (_syncLock)
            {
                var server = Values.FirstOrDefault(x => x.BattlesRunning < x.MaxBattles);
                return server;
            }
        }
    }
}