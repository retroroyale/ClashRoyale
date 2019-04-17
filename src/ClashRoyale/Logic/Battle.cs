using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using ClashRoyale.Protocol.Messages.Server;
using SharpRaven.Data;

namespace ClashRoyale.Logic
{
    public class Battle : List<Player>
    {
        private readonly Timer _battleTimer = new Timer(500)
        {
            AutoReset = true
        };

        public Dictionary<long, Queue<byte[]>> Commands = new Dictionary<long, Queue<byte[]>>();
        public long BattleId { get; set; }

        private DateTime StartTime { get; set; }

        public int BattleTime => (int) DateTime.UtcNow.Subtract(StartTime).TotalSeconds * 2;
        public int BattleSeconds => BattleTime / 2;

        public bool IsReady => Count >= 1;
        public bool Is1Vs1 { get; set; }

        public Battle(bool is1Vs1)
        {
            Is1Vs1 = is1Vs1;
        }

        public async void Start()
        {
            if (!IsReady) return;

            try
            {
                foreach (var player in this)
                {
                    Commands.Add(player.Home.Id, new Queue<byte[]>());
                }

                _battleTimer.Elapsed += BattleTick;

                foreach (var player in this)
                {
                    await new SectorStateMessage(player.Device)
                    {
                        Player1 = this[1],
                        Player2 = this[0]
                    }.SendAsync();
                }

                StartTime = DateTime.UtcNow;

                ForEach(p => p.Device.LastSectorCommand = DateTime.UtcNow);

                _battleTimer.Start();
            }
            catch (Exception)
            {
                Logger.Log("Couldn't start battle", GetType(), ErrorLevel.Error);
            }
        }

        public void Stop()
        {
            _battleTimer.Stop();

            Resources.Battles.Remove(BattleId);
        }

        public async void BattleTick(object sender, ElapsedEventArgs args)
        {
            try
            {
                foreach (var player in ToArray())
                    if (player.Device.IsConnected)
                    {
                        if (player.Device.SecondsSinceLastCommand > 2)
                        {
                            if (BattleSeconds > 10)
                            {
                                if(Is1Vs1)
                                    player.Home.AddCrowns(3);

                                await new BattleResultMessage(player.Device).SendAsync();

                                Remove(player);
                            }
                        }
                        else
                        {
                            await new SectorHearbeatMessage(player.Device)
                            {
                                Turn = BattleTime,
                                Commands = GetOwnQueue(player.Home.Id)
                            }.SendAsync();
                        }
                    }
                    else
                    {
                        Remove(player);
                    }

                if (FindIndex(p => p.Device.SecondsSinceLastCommand < 10) <= -1)
                    Stop();
            }
            catch (Exception)
            {
                Logger.Log("BattleTick failed.", GetType(), ErrorLevel.Error);
            }
        }

        public new void Remove(Player player)
        {
            if (Count <= 1)
                Stop();

            base.Remove(player);
        }

        public Device GetEnemy(long userId)
        {
            return this.FirstOrDefault(p => p.Home.Id != userId)?.Device;
        }

        public Queue<byte[]> GetEnemyQueue(long userId)
        {
            return Commands.FirstOrDefault(cmd => cmd.Key != userId).Value;
        }

        public Queue<byte[]> GetOwnQueue(long userId)
        {
            return Commands.FirstOrDefault(cmd => cmd.Key == userId).Value;
        }
    }
}