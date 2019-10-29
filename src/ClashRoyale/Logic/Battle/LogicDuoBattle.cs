using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using ClashRoyale.Files;
using ClashRoyale.Files.CsvLogic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;
using SharpRaven.Data;

namespace ClashRoyale.Logic.Battle
{
    public class LogicDuoBattle : List<Player>
    {
        /// <summary>
        ///     2v2 Battle
        /// </summary>
        /// <param name="arena"></param>
        /// <param name="players"></param>
        public LogicDuoBattle(int arena, IReadOnlyCollection<Player> players)
        {
            if (players.Count < 4)
            {
                Logger.Log("Not enough players to start a 2v2 battle.", GetType(), ErrorLevel.Error);
                return;
            }

            Arena = arena;
            Location = Csv.Tables.Get(Csv.Files.Locations)
                           .GetData<Locations>(Csv.Tables.Get(Csv.Files.Arenas)
                               .GetDataWithInstanceId<Arenas>(Arena - 1).TeamVsTeamLocation).GetInstanceId() +
                       1;

            AddRange(players);

            BattleTimer = new Timer(500);
            BattleTimer.Elapsed += Tick;
        }

        public int BattleTime => (int) DateTime.UtcNow.Subtract(StartTime).TotalSeconds * 2;
        public int BattleSeconds => BattleTime / 2;

        public bool IsReady => Count >= 4;

        public async void Start()
        {
            if (!IsReady) return;

            try
            {
                foreach (var player in this)
                {
                    Commands.Add(new Queue<byte[]>());

                    await new DuoSectorStateMessage(player.Device)
                    {
                        Battle = this
                    }.SendAsync();
                }

                StartTime = DateTime.UtcNow;

                BattleTimer.Start();
            }
            catch (Exception)
            {
                Logger.Log("Couldn't start battle", GetType(), ErrorLevel.Error);
            }
        }

        public void Encode(IByteBuffer packet)
        {
            #region SectorState



            #endregion SectorState
        }

        /// <summary>
        ///     Stops the battle
        /// </summary>
        public void Stop()
        {
            BattleTimer.Stop();

            Resources.DuoBattles.Remove(BattleId);
        }

        /// <summary>
        ///     Checks wether the battle is over or we have to send sector heartbeat (TCP only)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public async void Tick(object sender, ElapsedEventArgs args)
        {
            try
            {
                foreach (var player in this)
                    if(player != null)
                        if (player.Device.IsConnected)
                        {
                            if (player.Device.SecondsSinceLastCommand > 2)
                            {
                                if (BattleSeconds <= 8) continue;

                                if (!IsFriendly)
                                {
                                    player.Home.AddCrowns(3);
                                    player.Home.Arena.AddTrophies(31);
                                }

                                await new BattleResultMessage(player.Device).SendAsync();

                                Remove(player);
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

                if (FindIndex(p => p?.Device.SecondsSinceLastCommand < 10) <= -1)
                    Stop();
            }
            catch (Exception)
            {
                Logger.Log("DuoBattleTick failed.", GetType(), ErrorLevel.Error);
                Stop();
            }
        }

        /// <summary>
        ///     Remove a player from the battle and stop it when it's empty
        /// </summary>
        /// <param name="player"></param>
        public new void Remove(Player player)
        {
            if (Count <= 1)
                Stop();

            var index = FindIndex(x => x.Home.Id == player.Home.Id);

            if (index > -1)
            {
                player.DuoBattle = null;
                this[index] = null;
            }
        }

        public IEnumerator<Device> GetEnemies(long userId)
        {
            if (FindIndex(x => x.Home.Id == userId) == 0)
            {
                yield return this[1].Device;
                yield return this[3].Device;
            }
            else
            {
                yield return this[0].Device;
                yield return this[2].Device;
            }
        }

        public List<Player> GetAllOthers(long userId)
        {
            return this.Where(x => x.Home.Id != userId).ToList();
        }

        public IEnumerator<Queue<byte[]>> GetEnemyQueues(long userId)
        {
            if (FindIndex(x => x.Home.Id == userId) == 0)
            {
                yield return Commands[1];
                yield return Commands[3];
            }
            else
            {
                yield return Commands[0];
                yield return Commands[2];
            }
        }

        public IEnumerator<Queue<byte[]>> GetOwnQueues(long userId)
        {
            if (FindIndex(x => x.Home.Id == userId) == 0)
            {
                yield return Commands[0];
                yield return Commands[2];
            }
            else
            {
                yield return Commands[1];
                yield return Commands[3];
            }
        }

        public Queue<byte[]> GetOwnQueue(long userId)
        {
            var index = FindIndex(x => x.Home.Id == userId);

            if (index > -1)
            { 
                return Commands[index];
            }

            return new Queue<byte[]>();
        }

        #region Objects

        public List<Queue<byte[]>> Commands = new List<Queue<byte[]>>();
        public Timer BattleTimer;
        public bool IsFriendly { get; set; }
        public int Arena { get; set; }
        public long BattleId { get; set; }
        public int Location { get; set; }
        private DateTime StartTime { get; set; }

        public static int[] KingTowerHp =
        {
            2400, 2568, 2736, 2904, 3096, 3312, 3528, 3768, 4008, 4392, 4824, 5304, 5832
        };

        public static int[] PrincessTowerHp =
        {
            1400, 1512, 1624, 1750, 1890, 2030, 2184, 2352, 2534, 2786, 3052, 3346, 3668
        };

        #endregion
    }
}