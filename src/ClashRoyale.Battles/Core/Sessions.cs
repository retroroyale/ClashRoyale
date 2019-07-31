using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using ClashRoyale.Battles.Logic.Session;
using SharpRaven.Data;

namespace ClashRoyale.Battles.Core
{
    public class Sessions : Dictionary<long, Session>
    {
        private readonly object _syncObject = new object();
        private readonly Timer _timer = new Timer(10000);

        public Sessions()
        {
            _timer.Elapsed += TimerCallback;
            _timer.Start();
        }

        public void Add(SessionContext ctx, long sessionId)
        {
            lock (_syncObject)
            {
                if (!ContainsKey(sessionId))
                {
                    var session = new Session
                    {
                        Id = sessionId
                    };

                    session.Add(ctx);
                    ctx.Session = session;

                    Add(sessionId, session);
                }
                else
                {
                    var session = Get(sessionId);

                    if (session.Count < 2)
                    {
                        session.Add(ctx);
                        ctx.Session = session;
                    }
                    else
                    {
                        // since the tcp server can be restarted and the id resets
                        Remove(sessionId);
                        Add(ctx, sessionId);
                    }
                }
            }
        }

        public new void Remove(long sessionId)
        {
            lock (_syncObject)
            {
                if (ContainsKey(sessionId))
                {
                    base.Remove(sessionId);
                }
            }
        }

        public Session Get(long sessionId)
        {
            lock (_syncObject)
            {
                return ContainsKey(sessionId) ? this[sessionId] : null;
            }
        }

        public void TimerCallback(object state, ElapsedEventArgs args)
        {
            try
            {
                lock (_syncObject)
                {
                    foreach (var session in Values.ToArray())
                    {
                        foreach (var ctx in session.ToArray())
                        {
                            if (ctx.Active) continue;
                            session.Remove(ctx);
                        }

                        if (session.Count == 0)
                        {
                            Remove(session.Id);
                        }
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
