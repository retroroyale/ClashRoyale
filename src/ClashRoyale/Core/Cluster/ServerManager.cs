using System.Collections.Generic;

namespace ClashRoyale.Core.Cluster
{
    public class ServerManager : Dictionary<string, ServerInfo>
    {
        private readonly object _syncLock = new object();

        public void Add(string host)
        {
            lock (_syncLock)
            {
                if (host.Contains(':'))
                {

                }
            }
        }

        public void Remove(string host)
        {
            lock (_syncLock)
            {
                // TODO
            }
        }
    }
}