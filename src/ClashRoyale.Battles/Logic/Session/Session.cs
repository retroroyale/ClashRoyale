using System.Collections.Generic;
using System.Net;
using ClashRoyale.Battles.Logic.Battle;

namespace ClashRoyale.Battles.Logic.Session
{
    public class Session : List<SessionContext>
    {
        private readonly object _syncObject = new object();

        public Session()
        {
            Battle = new LogicBattle(this);
        }

        public long Id { get; set; }
        public LogicBattle Battle { get; set; }

        public new void Add(SessionContext ctx)
        {
            lock (_syncObject)
            {
                if (!Contains(ctx))
                {
                    base.Add(ctx);

                    if (Count >= 2) Battle.Start();
                }
            }
        }

        public new void Remove(SessionContext ctx)
        {
            lock (_syncObject)
            {
                if (!Contains(ctx)) return;

                base.Remove(ctx);

                if (Count < 1) Battle.Stop();
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