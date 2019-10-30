using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using ClashRoyale.Extensions;
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

            const int towers = 10;

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

            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);

            packet.WriteVInt(84);
            packet.WriteVInt(84);
            packet.WriteVInt(0);
            packet.WriteVInt(0);

            packet.WriteVInt(towers);
            packet.WriteVInt(towers);

            // KingTower
            packet.WriteData(Csv.Tables.Get(Csv.Files.Buildings).GetDataWithInstanceId<Buildings>(1));
            packet.WriteData(Csv.Tables.Get(Csv.Files.Buildings).GetDataWithInstanceId<Buildings>(1));
            packet.WriteData(Csv.Tables.Get(Csv.Files.Buildings).GetDataWithInstanceId<Buildings>(1));
            packet.WriteData(Csv.Tables.Get(Csv.Files.Buildings).GetDataWithInstanceId<Buildings>(1));

            // PrincessTower
            packet.WriteData(Csv.Tables.Get(Csv.Files.Buildings).GetDataWithInstanceId<Buildings>(0));
            packet.WriteData(Csv.Tables.Get(Csv.Files.Buildings).GetDataWithInstanceId<Buildings>(0));
            packet.WriteData(Csv.Tables.Get(Csv.Files.Buildings).GetDataWithInstanceId<Buildings>(0));
            packet.WriteData(Csv.Tables.Get(Csv.Files.Buildings).GetDataWithInstanceId<Buildings>(0));

            // KingTowerMiddle
            packet.WriteData(Csv.Tables.Get(Csv.Files.Buildings).GetDataWithInstanceId<Buildings>(16));
            packet.WriteData(Csv.Tables.Get(Csv.Files.Buildings).GetDataWithInstanceId<Buildings>(16));

            // LogicGameObject::encodeComponent
            packet.WriteVInt(1);
            packet.WriteVInt(2);
            packet.WriteVInt(3);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(1);
            packet.WriteVInt(2);
            packet.WriteVInt(3);
            packet.WriteVInt(1);
            packet.WriteVInt(0);

            for (var i = 0; i < towers; i++)
            {
                packet.WriteVInt(5);
                packet.WriteVInt(i);
            }

            packet.WriteVInt(7);
            packet.WriteVInt(13);
            packet.WriteVInt(14500);
            packet.WriteVInt(25500);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(-1);
            packet.WriteVInt(0);
            packet.WriteVInt(-1);
            packet.WriteVInt(0);
            packet.WriteVInt(2);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);

            packet.WriteVInt(0);

            packet.WriteVInt(7);
            packet.WriteVInt(13);
            packet.WriteVInt(3500);
            packet.WriteVInt(6500);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(-1);
            packet.WriteVInt(0);
            packet.WriteVInt(-1);
            packet.WriteVInt(0);
            packet.WriteVInt(1);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);

            var home1 = this[0];
            var home2 = this[2];

            var enemy1 = this[1];
            var enemy2 = this[3];

            packet.WriteVInt(0);
            packet.WriteVInt(7);
            packet.WriteVInt(13);
            packet.WriteVInt(3500);
            packet.WriteVInt(25500);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(-1);
            packet.WriteVInt(0);
            packet.WriteVInt(-1);
            packet.WriteVInt(0);
            packet.WriteVInt(1);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);

            packet.WriteVInt(0);
            packet.WriteVInt(7);
            packet.WriteVInt(13);
            packet.WriteVInt(14500);
            packet.WriteVInt(6500);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(-1);
            packet.WriteVInt(0);
            packet.WriteVInt(-1);
            packet.WriteVInt(0);
            packet.WriteVInt(2);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);

            // Home
            packet.WriteVInt(0);
            packet.WriteVInt(7);
            packet.WriteVInt(13);
            packet.WriteVInt(11000);
            packet.WriteVInt(3000);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(-1);
            packet.WriteVInt(0);
            packet.WriteVInt(-1);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(2);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(5);

            // Rotation
            packet.WriteByte(4);
            packet.WriteByte(0);
            packet.WriteByte(1);
            packet.WriteByte(1);
            packet.WriteByte(1);

            packet.WriteByte(4);
            for (var i = 4; i < 8; i++)
                packet.WriteByte(i);

            packet.WriteHex("007F7F00000005");

            packet.WriteHex(
                "00000000007F7F7F7F7F7F7F7F00");

            // Enemy
            packet.WriteVInt(7);
            packet.WriteVInt(13);
            packet.WriteVInt(11000);
            packet.WriteVInt(29000);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(-1);
            packet.WriteVInt(0);
            packet.WriteVInt(-1);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(2);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(5);

            // Rotation
            packet.WriteByte(4);
            packet.WriteByte(0);
            packet.WriteByte(1);
            packet.WriteByte(1);
            packet.WriteByte(1);

            packet.WriteByte(4);
            for (var i = 4; i < 8; i++)
                packet.WriteByte(i);

            packet.WriteHex("007F7F00000005");

            packet.WriteHex(
                "00000000007F7F7F7F7F7F7F7F00");

            // Home
            packet.WriteVInt(7);
            packet.WriteVInt(13);
            packet.WriteVInt(7000);
            packet.WriteVInt(3000);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(-1);
            packet.WriteVInt(0);
            packet.WriteVInt(-1);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(1);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(5);

            // Rotation
            packet.WriteByte(4);
            packet.WriteByte(0);
            packet.WriteByte(1);
            packet.WriteByte(1);
            packet.WriteByte(1);

            packet.WriteByte(4);
            for (var i = 4; i < 8; i++)
                packet.WriteByte(i);

            packet.WriteHex("007F7F00000005");

            packet.WriteHex(
                "00000000007F7F7F7F7F7F7F7F00");

            // Home
            packet.WriteVInt(7);
            packet.WriteVInt(13);
            packet.WriteVInt(7000);
            packet.WriteVInt(29000);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(-1);
            packet.WriteVInt(0);
            packet.WriteVInt(-1);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(1);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(5);

            // Rotation
            packet.WriteByte(4);
            packet.WriteByte(0);
            packet.WriteByte(1);
            packet.WriteByte(1);
            packet.WriteByte(1);

            packet.WriteByte(4);
            for (var i = 4; i < 8; i++)
                packet.WriteByte(i);

            packet.WriteHex("007F7F00000005");

            packet.WriteHex(
                "00000000007F7F7F7F7F7F7F7F00");

            packet.WriteVInt(0);
            packet.WriteVInt(9);
            packet.WriteVInt(9000);
            packet.WriteVInt(29000);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(-1);
            packet.WriteVInt(0);
            packet.WriteVInt(-1);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);

            packet.WriteVInt(0);
            packet.WriteVInt(9);
            packet.WriteVInt(9000);
            packet.WriteVInt(3000);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(-1);
            packet.WriteVInt(0);
            packet.WriteVInt(-1);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);

            packet.WriteHex(
                "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");

            // LogicHitpointComponent
            packet.WriteVInt(2352);
            packet.WriteVInt(0);

            packet.WriteVInt(2352);
            packet.WriteVInt(0);

            packet.WriteVInt(2352);
            packet.WriteVInt(0);

            packet.WriteVInt(2352);
            packet.WriteVInt(0);

            packet.WriteVInt(4522);
            packet.WriteVInt(0);

            packet.WriteVInt(4522);
            packet.WriteVInt(0);

            packet.WriteVInt(4522);
            packet.WriteVInt(0);

            packet.WriteVInt(4522);
            packet.WriteVInt(0);

            for (var i = 0; i < towers; i++)
                packet.WriteHex("00000000000000A401A401");

            packet.WriteHex("FF01");
            home1.Home.Deck.EncodeAttack(packet);

            packet.WriteVInt(0);
            packet.WriteHex("FE01");
            home2.Home.Deck.EncodeAttack(packet);

            packet.WriteVInt(0);
            packet.WriteHex("FE03");
            enemy1.Home.Deck.EncodeAttack(packet);

            packet.WriteVInt(0);
            packet.WriteHex("FE03");
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
                foreach (var player in ToArray())
                    if (player != null)
                        if (player.Device.IsConnected)
                        {
                            if (player.Device.SecondsSinceLastCommand > 2)
                            {
                                if (BattleSeconds <= 8) continue;

                                if (!IsFriendly) player.Home.AddCrowns(3);

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
            catch (Exception exc)
            {
                Logger.Log($"DuoBattleTick failed. {exc}", GetType(), ErrorLevel.Error);
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

            if (player == null) return;

            var index = FindIndex(x => x?.Home.Id == player.Home.Id);

            if (index <= -1) return;

            player.DuoBattle = null;
            this[index] = null;
        }

        public Player GetTeammate(long userId)
        {
            var index = FindIndex(x => x.Home.Id == userId);
            return this[index % 2 == 0 ? index == 0 ? 2 : 0 : index == 1 ? 3 : 1];
        }

        public List<Player> GetAllOthers(long userId)
        {
            return this.Where(x => x?.Home.Id != userId).ToList();
        }

        public Queue<byte[]> GetOwnQueue(long userId)
        {
            var index = FindIndex(x => x?.Home.Id == userId);
            return index > -1 ? Commands[index] : new Queue<byte[]>();
        }

        public List<Queue<byte[]>> GetOtherQueues(long userId)
        {
            var index = FindIndex(x => x.Home.Id == userId);
            var cmd = Commands.ToList();

            cmd.RemoveAt(index);

            return cmd;
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
            2880, 3082, 3284, 3485, 3716, 3975, 4234, 4522, 4810, 5271, 5789, 6365, 6999
        };

        public static int[] PrincessTowerHp =
        {
            1400, 1512, 1624, 1750, 1890, 2030, 2184, 2352, 2534, 2786, 3052, 3346, 3668
        };

        #endregion
    }
}