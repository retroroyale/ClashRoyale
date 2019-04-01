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

        public Queue<byte[]> Commands1 = new Queue<byte[]>();
        public Queue<byte[]> Commands2 = new Queue<byte[]>();
        public long BattleId { get; set; }

        private DateTime StartTime { get; set; }

        public int BattleTime => (int) DateTime.UtcNow.Subtract(StartTime).TotalSeconds * 2;
        public int BattleSeconds => BattleTime / 2;

        public bool IsReady => Count >= 1;

        public async void Start()
        {
            if (!IsReady) return;

            try
            {
                _battleTimer.Elapsed += BattleTick;

                var s1 = new SectorStateMessage(this[0].Device)
                {
                    Player1 = this[1],
                    Player2 = this[0]
                };

                var s2 = new SectorStateMessage(this[1].Device)
                {
                    Player1 = this[1],
                    Player2 = this[0]
                };

                await s1.Send();
                await s2.Send();

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
                                player.Home.AddCrowns(3);

                                await new BattleEndMessage(player.Device).Send();

                                Remove(player);
                            }
                        }
                        else
                        {
                            await new SectorHearbeatMessage(player.Device)
                            {
                                Turn = BattleTime,
                                Commands = GetOwnQueue(player.Home.Id)
                            }.Send();
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
            var index = FindIndex(p => p.Home.Id == userId);

            switch (index)
            {
                case 0:
                {
                    return Commands2;
                }

                case 1:
                {
                    return Commands1;
                }
            }

            return null;
        }

        public Queue<byte[]> GetOwnQueue(long userId)
        {
            var index = FindIndex(p => p.Home.Id == userId);

            switch (index)
            {
                case 0:
                {
                    return Commands1;
                }

                case 1:
                {
                    return Commands2;
                }
            }

            return null;
        }
    }
}