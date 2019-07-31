using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Timers;
using ClashRoyale.Battles.Protocol.Messages.Server;
using SharpRaven.Data;

namespace ClashRoyale.Battles.Logic.Battle
{
    public class LogicBattle
    {
        public Timer BattleTimer;
        public Session.Session Session { get; set; }

        public Dictionary<EndPoint, Queue<byte[]>> Commands = new Dictionary<EndPoint, Queue<byte[]>>();

        public LogicBattle(Session.Session session)
        {
            Session = session;

            BattleTimer = new Timer(500);
            BattleTimer.Elapsed += Tick;
        }

        private DateTime StartTime { get; set; }

        public int BattleTime => (int) DateTime.UtcNow.Subtract(StartTime).TotalSeconds * 2;
        public int BattleSeconds => BattleTime / 2;

        public bool IsRunning => BattleTimer.Enabled;
        public bool IsReady => Session.Count >= 1;

        public void Start()
        {
            if (!IsReady) return;

            try
            {
                foreach (var session in Session)
                {
                    Commands.Add(session.EndPoint, new Queue<byte[]>());
                }

                StartTime = DateTime.UtcNow;
                BattleTimer.Start();
            }
            catch (Exception)
            {
                Logger.Log("Couldn't start battle", GetType(), ErrorLevel.Error);
            }
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
                foreach (var session in Session.ToArray())
                    if (session.Active)
                    {
                        if (!session.BattleActive)
                        {
                            if (BattleSeconds <= 8) continue;

                            //await new BattleResultMessage(player.Device).SendAsync();

                            Logger.Log("BATTLE OVER", null);
                        }
                        else
                        {
                            await new SectorHearbeatMessage(session)
                            {
                                Turn = BattleTime,
                                Commands = GetOwnQueue(session.EndPoint)
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
    }
}