using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using ClashRoyale.Files;
using ClashRoyale.Files.CsvLogic;
using ClashRoyale.Protocol.Messages.Server;
using ClashRoyale.Utilities.Netty;
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
                for (var i = 0; i < Count; i++)
                {
                    var player = this[i];

                    Commands.Add(new Queue<byte[]>());

                    await new DuoSectorStateMessage(player.Device)
                    {
                        Battle = this,
                        EnemyTeam = i == 1 || i == 3
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

        public void Encode(IByteBuffer packet, bool enemy)
        {
            #region SectorState

            packet.WriteVInt(Location); // LocationData

            packet.WriteVInt(Count); // PlayerCount
            packet.WriteVInt(0); // NpcData
            packet.WriteVInt(Arena); // ArenaData

            for (var i = 0; i < Count; i++)
            {
                packet.WriteVInt(this[i].Home.HighId);
                packet.WriteVInt(this[i].Home.LowId);
                packet.WriteVInt(0);
            }

            packet.WriteHex(
                "000000000000000000000000009401EC7E00000A0A2301230123012301230023002300230023102310010203000001020301000500050105020503050405050506050705080509070DA4E2019C8E0300007F00C07C0002000000000000070DAC36A46500007F0080040001000000000000070DAC369C8E0300007F00C07C0001000000000000070DA4E201A46500007F0080040002000000000000070DB8AB01B82E00007F00800400000200000000000005");

            var home1 = this[0];
            var home2 = this[2];

            var enemy1 = this[1];
            var enemy2 = this[3];

            packet.WriteByte(4);
            for (var i = 0; i < 4; i++)
                packet.WriteByte(i);

            packet.WriteByte(4);
            for (var i = 4; i < 8; i++)
                packet.WriteByte(i);

            packet.WriteHex(
                "007F7F0000000500000000007F7F7F7F7F7F7F7F00070DB8AB0188C50300007F00C07C0000020000000000000400000500000000007F7F7F7F7F7F7F7F00070D986DB82E00007F00800400000100000000000005");

            packet.WriteByte(4);
            for (var i = 0; i < 4; i++)
                packet.WriteByte(i);

            packet.WriteByte(4);
            for (var i = 4; i < 8; i++)
                packet.WriteByte(i);

            packet.WriteHex(
                "007F7F0000000500000000007F7F7F7F7F7F7F7F00070D986D88C50300007F00C07C0000010000000000000400000500000000007F7F7F7F7F7F7F7F000009A88C0188C50300007F00C07C00000000000000000009A88C01B82E00007F00800400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B02400B02400B02400B02400AA4600AA4600AA4600AA460000000000000000A401A40100000000000000A401A40100000000000000A401A40100000000000000A401A40100000000000000A401A40100000000000000A401A40100000000000000A401A40100000000000000A401A40100000000000000A401A40100000000000000A401A401");

            packet.WriteHex("FF01");

            if (enemy)
                home1.Home.Deck.EncodeAttack(packet);
            else
                enemy1.Home.Deck.EncodeAttack(packet);

            packet.WriteVInt(0);
            packet.WriteHex("FE01");

            if(enemy)
                home2.Home.Deck.EncodeAttack(packet);
            else
                enemy2.Home.Deck.EncodeAttack(packet);

            packet.WriteHex("00000506070802040202010300000000000000010200001800000C000000CCE9D7B507002A002B");

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

        public List<Player> GetAllOthers(long userId)
        {
            return this.Where(x => x?.Home.Id != userId).ToList();
        }

        public List<Queue<byte[]>> GetOtherQueues(long userId)
        {
            var index = FindIndex(x => x.Home.Id == userId);
            var cmd = Commands.ToList();

            cmd.RemoveAt(index);

            return cmd;
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