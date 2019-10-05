using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Timers;
using ClashRoyale.Battles.Core.Network.Cluster.Protocol.Messages.Client;
using ClashRoyale.Battles.Protocol.Messages.Server;
using ClashRoyale.Utilities.Models.Battle.Replay;
using Newtonsoft.Json;
using SharpRaven.Data;

namespace ClashRoyale.Battles.Logic.Battle
{
    public class LogicBattle
    {
        public LogicBattle(Session.Session session)
        {
            Session = session;

            BattleTimer = new Timer(500);
            BattleTimer.Elapsed += Tick;
        }

        public int BattleTime => (int) DateTime.UtcNow.Subtract(StartTime).TotalSeconds * 2;
        public int BattleSeconds => BattleTime / 2;

        public bool IsReady => Session.Count >= 1;

        public void Start()
        {
            if (!IsReady) return;

            foreach (var session in Session) Commands.Add(session.EndPoint, new Queue<byte[]>());

            StartTime = DateTime.UtcNow;
            BattleTimer.Start();
        }

        public void Stop()
        {
            BattleTimer.Stop();

            Resources.Sessions.Remove(Session.Id);
        }

        public async void Tick(object sender, ElapsedEventArgs args)
        {
            try
            {
                foreach (var ctx in Session.ToArray())
                    if (ctx.Active)
                    {
                        if (DateTime.UtcNow.Subtract(ctx.LastCommands).TotalSeconds > 3)
                        {
                            if (BattleSeconds <= 10) continue;

                            await new BattleFinishedMessage
                            {
                                SessionId = Session.Id,
                                Index = ctx.Index,
                                ReplayJson = JsonConvert.SerializeObject(Replay)
                            }.SendAsync();

                            ctx.Session.Remove(ctx);
                        }
                        else
                        {
                            await new SectorHearbeatMessage(ctx)
                            {
                                Turn = BattleTime,
                                Commands = GetOwnQueue(ctx.EndPoint)
                            }.SendAsync();
                        }
                    }

                if (Session.FindIndex(s => s.BattleActive) <= -1)
                    Stop();
            }
            catch (Exception)
            {
                Logger.Log("BattleTick failed.", GetType(), ErrorLevel.Error);
            }
        }

        public Queue<byte[]> GetEnemyQueue(EndPoint endpoint)
        {
            return Commands.FirstOrDefault(cmd => cmd.Key != endpoint).Value;
        }

        public Queue<byte[]> GetOwnQueue(EndPoint endpoint)
        {
            return Commands.FirstOrDefault(cmd => cmd.Key == endpoint).Value;
        }

        #region Objects 

        private DateTime StartTime { get; set; }
        public Timer BattleTimer;
        public Dictionary<EndPoint, Queue<byte[]>> Commands = new Dictionary<EndPoint, Queue<byte[]>>();
        public Session.Session Session { get; set; }
        public LogicReplay Replay = new LogicReplay();

        #endregion
    }
}