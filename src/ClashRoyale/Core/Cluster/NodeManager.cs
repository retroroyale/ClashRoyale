using System.Collections.Generic;
using System.Linq;

namespace ClashRoyale.Core.Cluster
{
    public class NodeManager : Dictionary<string, NodeInfo>
    {
        private readonly object _syncLock = new object();

        /// <summary>
        ///     Add a new server
        /// </summary>
        /// <param name="host"></param>
        /// <param name="info"></param>
        public new void Add(string host, NodeInfo info)
        {
            lock (_syncLock)
            {
                if (host.Contains(':')) base.Add(host, info);
            }
        }

        /// <summary>
        ///     Remove a server by the host
        /// </summary>
        /// <param name="host"></param>
        public new void Remove(string host)
        {
            lock (_syncLock)
            {
                if (!host.Contains(':')) return;

                if (ContainsKey(host)) base.Remove(host);
            }
        }

        /// <summary>
        ///     Returns a server if available with the lowest battles running
        /// </summary>
        /// <returns></returns>
        public NodeInfo GetServer()
        {
            lock (_syncLock)
            {
                var server = Values.FirstOrDefault(x => x.BattlesRunning < x.MaxBattles);
                return server;
            }
        }
    }
}