using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using ClashRoyale.Battles.Logic.Session;
using SharpRaven.Data;

namespace ClashRoyale.Battles.Core
{
    public class Players : Dictionary<long, SessionContext>
    {
        private readonly object _syncObject = new object();
        private readonly Timer _timer = new Timer(10000);

        public Players()
        {
            _timer.Elapsed += TimerCallback;
            _timer.Start();
        }

        public void Add(SessionContext ctx)
        {
            Remove(ctx.PlayerId);

            lock (_syncObject)
            {
                if (!ContainsKey(ctx.PlayerId))
                {
                    Add(ctx.PlayerId, ctx);
                }
            }
        }

        public new void Remove(long playerId)
        {
            lock (_syncObject)
            {
                if (ContainsKey(playerId))
                {
                    base.Remove(playerId);
                }
            }
        }

        public SessionContext Get(long playerId)
        {
            lock (_syncObject)
            {
                return ContainsKey(playerId) ? this[playerId] : null;
            }
        }

        public void TimerCallback(object state, ElapsedEventArgs args)
        {
            try
            {
                lock (_syncObject)
                {
                    foreach (var ctx in Values.ToArray())
                    {
                        if (ctx.Active) continue;

                        Remove(ctx.PlayerId);
                        Logger.Log($"Removed player {ctx.PlayerId}", null);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Log(exception, null, ErrorLevel.Debug);
            }
        }
    }
}
