using System.Collections.Generic;
using System.Net;

namespace ClashRoyale.Battles.Logic.Session
{
    public class Session : List<SessionContext>
    {
        private readonly object _syncObject = new object();

        public long Id { get; set; }

        public new void Add(SessionContext ctx)
        {
            lock (_syncObject)
            {
                if (!Contains(ctx))
                {
                    base.Add(ctx);
                }
            }
        }

        public SessionContext Get(EndPoint endPoint)
        {
            lock (_syncObject)
            {
                var index = FindIndex(x => x.EndPoint.ToString() == endPoint.ToString());
                return index > -1 ? this[index] : null;
            }
        }
    }
}
