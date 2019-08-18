using System;
using System.Collections.Generic;
using System.Timers;
using ClashRoyale.Core.Cluster;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;
using SharpRaven.Data;

namespace ClashRoyale.Logic.Battle
{
    public class LogicDuoBattle
    {
        public Timer BattleTimer;

        public Dictionary<long, Queue<byte[]>> Commands = new Dictionary<long, Queue<byte[]>>();

        public List<List<Player>> Teams = new List<List<Player>>();

        /// <summary>
        ///     2v2 Battle
        /// </summary>
        /// <param name="arena"></param>
        /// <param name="players"></param>
        public LogicDuoBattle(int arena, IReadOnlyList<Player> players)
        {
            if (players.Count < 4)
                return;

            Arena = arena;

            for (var i = 0; i < 2; i++) Teams.Add(new List<Player>());

            for (var i = 0; i < 4; i++) Teams[i % 2 == 0 ? 0 : 1].Add(players[i]);

            BattleTimer = new Timer(500);
            BattleTimer.Elapsed += Tick;
        }

        public long BattleId { get; set; }

        private DateTime StartTime { get; set; }

        public int BattleTime => (int) DateTime.UtcNow.Subtract(StartTime).TotalSeconds * 2;
        public int BattleSeconds => BattleTime / 2;

        public bool IsReady => Teams.Count >= 2;
        public bool IsFriendly { get; set; }
        public int Arena { get; set; }

        public async void Start()
        {
            if (!IsReady) return;

            try
            {
                NodeInfo server = null;
                if (Resources.Configuration.UseUdp) server = Resources.NodeManager.GetServer();

                foreach (var team in Teams)
                foreach (var player in team)
                {
                    Commands.Add(player.Home.Id, new Queue<byte[]>());

                    if (Resources.Configuration.UseUdp)
                        if (server != null)
                            await new UdpConnectionInfoMessage(player.Device)
                            {
                                ServerPort = server.Port,
                                ServerHost = server.Ip == "127.0.0.1" ? "192.168.2.143" : server.Ip, // just as test
                                SessionId = BattleId,
                                Nonce = server.Nonce
                            }.SendAsync();

                    await new DuoSectorStateMessage(player.Device)
                    {
                        Battle = this
                    }.SendAsync();
                }

                StartTime = DateTime.UtcNow;

                if (!Resources.Configuration.UseUdp || server == null)
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

            /*
            const int towers = 6;

            packet.WriteVInt(Csv.Tables.Get(Csv.Files.Locations)
                                 .GetData<Locations>(Csv.Tables.Get(Csv.Files.Arenas)
                                     .GetDataWithInstanceId<Arenas>(Arena - 1).PvpLocation).GetInstanceId() +
                             1); // LocationData

            packet.WriteVInt(Count); // PlayerCount
            packet.WriteVInt(0); // NpcData
            packet.WriteVInt(Arena); // ArenaData

            foreach (var player in this)
            {
                packet.WriteVInt(player.Home.HighId);
                packet.WriteVInt(player.Home.LowId);
                packet.WriteVInt(0);
            }

            // ConstantSizeIntArray
            {
                packet.WriteVInt(1);
                packet.WriteVInt(0);
                packet.WriteVInt(0);

                packet.WriteVInt(7);
                packet.WriteVInt(0);
                packet.WriteVInt(0);
            }

            packet.WriteBoolean(false); // IsReplay / Type?
            packet.WriteBoolean(false); // IsEndConditionMatched
            packet.WriteBoolean(false);

            packet.WriteBoolean(false); // IsNpc

            packet.WriteBoolean(false); // isBattleEndedWithTimeOut
            packet.WriteBoolean(false);

            packet.WriteBoolean(false); // hasPlayerFinishedNpcLevel
            packet.WriteBoolean(false);

            packet.WriteBoolean(false); // isInOvertime
            packet.WriteBoolean(false); // isTournamentMode

            packet.WriteVInt(0);

            packet.WriteVInt(towers);
            packet.WriteVInt(towers);

            packet.WriteData(Csv.Tables.Get(Csv.Files.Buildings).GetDataWithInstanceId<Buildings>(1));
            packet.WriteData(Csv.Tables.Get(Csv.Files.Buildings).GetDataWithInstanceId<Buildings>(1));
            packet.WriteData(Csv.Tables.Get(Csv.Files.Buildings).GetDataWithInstanceId<Buildings>(1));
            packet.WriteData(Csv.Tables.Get(Csv.Files.Buildings).GetDataWithInstanceId<Buildings>(1));

            packet.WriteData(Csv.Tables.Get(Csv.Files.Buildings).GetDataWithInstanceId<Buildings>(0));
            packet.WriteData(Csv.Tables.Get(Csv.Files.Buildings).GetDataWithInstanceId<Buildings>(0));

            // LogicGameObject::encodeComponent
            packet.WriteVInt(1);
            packet.WriteVInt(0);
            packet.WriteVInt(1);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(1);

            for (var i = 0; i < towers; i++)
            {
                packet.WriteVInt(5);
                packet.WriteVInt(i);
            }

            // Player Right Princess Tower
            packet.WriteVInt(12);
            packet.WriteVInt(13);
            packet.WriteVInt(14500); // X
            packet.WriteVInt(25500); // Y
            packet.WriteHex("00007F00C07C0002000000000000");

            // Enemy Left Princess Tower
            packet.WriteVInt(12);
            packet.WriteVInt(13);
            packet.WriteVInt(3500); // X
            packet.WriteVInt(6500); // Y
            packet.WriteHex("00007F0080040001000000000000");

            // Player Left Princess Tower
            packet.WriteVInt(12);
            packet.WriteVInt(13);
            packet.WriteVInt(3500); // X
            packet.WriteVInt(25500); // Y
            packet.WriteHex("00007F00C07C0001000000000000");

            // Enemy Right Princess Tower
            packet.WriteVInt(12);
            packet.WriteVInt(13);
            packet.WriteVInt(14500); // X
            packet.WriteVInt(6500); // Y
            packet.WriteHex("00007F0080040002000000000000");

            // Enemy Crown Tower
            packet.WriteVInt(12);
            packet.WriteVInt(13);
            packet.WriteVInt(9000); // X
            packet.WriteVInt(3000); // Y
            packet.WriteHex("00007F0080040000000000000000");

            packet.WriteHex("000504077F7D7F0400050401007F7F0000");
            packet.WriteVInt(0); // Ms before regen mana
            packet.WriteVInt(6); // Mana Start 
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);

            packet.WriteHex("00007F7F7F7F7F7F7F7F00");

            // Player Crown Tower
            packet.WriteVInt(12);
            packet.WriteVInt(13);
            packet.WriteVInt(9000); // X
            packet.WriteVInt(29000); // Y
            packet.WriteHex("00007F00C07C0000000000000000");

            packet.WriteHex("00050401047D010400040706007F7F0000");
            packet.WriteVInt(0); // Ms before regen mana
            packet.WriteVInt(6); // Elexir Start Enemy
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);

            packet.WriteVInt(0);
            packet.WriteVInt(0);

            for (var index = 0; index < 8; index++)
                packet.WriteVInt(-1);

            for (var index = 0; index < 48; index++)
                packet.WriteVInt(0);

            // LogicHitpointComponent
            packet.WriteVInt(3668); // Enemy 
            packet.WriteVInt(0);
            packet.WriteVInt(3668); // Player
            packet.WriteVInt(0);
            packet.WriteVInt(3668); // Enemy
            packet.WriteVInt(0);
            packet.WriteVInt(3668); // Player
            packet.WriteVInt(0);
            packet.WriteVInt(5832); // Enemy
            packet.WriteVInt(0);
            packet.WriteVInt(5832); // Player
            packet.WriteVInt(0);

            // LogicCharacterBuffComponent
            for (var index = 0; index < towers; index++)
                packet.WriteHex("00000000000000A401A401");

            packet.WriteHex("FF01");
            this[0].Home.Deck.EncodeAttack(packet);

            packet.WriteVInt(0);

            packet.WriteHex("FE03");
            this[1].Home.Deck.EncodeAttack(packet);

            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(5);
            packet.WriteVInt(6);
            packet.WriteVInt(2);
            packet.WriteVInt(2);
            packet.WriteVInt(4);
            packet.WriteVInt(2);
            packet.WriteVInt(1);
            packet.WriteVInt(3);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(6);
            packet.WriteVInt(1);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(9);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(12);

            packet.WriteHex("000000F69686FF0A002A002B");

            packet.WriteVInt(0);
            packet.WriteVInt(13);
            packet.WriteVInt(14500);
            packet.WriteVInt(25500);
            packet.WriteHex("00007F00C07C0002000000000000");

            packet.WriteVInt(0);
            packet.WriteVInt(13);
            packet.WriteVInt(3500);
            packet.WriteVInt(6500);
            packet.WriteHex("00007F0080040001000000000000");

            packet.WriteVInt(0);
            packet.WriteVInt(13);
            packet.WriteVInt(3500);
            packet.WriteVInt(25500);
            packet.WriteHex("00007F00C07C0001000000000000");

            packet.WriteVInt(0);
            packet.WriteVInt(13);
            packet.WriteVInt(14500);
            packet.WriteVInt(6500);
            packet.WriteHex("00007F0080040002000000000000");

            packet.WriteVInt(0);
            packet.WriteVInt(13);
            packet.WriteVInt(9000);
            packet.WriteVInt(3000);
            packet.WriteHex("00007F0080040000000000000000");

            packet.WriteVInt(0);
            packet.WriteVInt(5);
            packet.WriteVInt(1);
            packet.WriteVInt(0);

            packet.WriteHex("7F000000007F7F0000000100000000007F7F7F7F7F7F7F7F");
            packet.WriteVInt(0);

            packet.WriteVInt(0);
            packet.WriteVInt(13);
            packet.WriteVInt(9000);
            packet.WriteVInt(29000);
            packet.WriteHex("00007F00C07C0000000000000000");

            packet.WriteVInt(0);
            packet.WriteVInt(5);
            packet.WriteVInt(4);
            packet.WriteVInt(0);
            packet.WriteVInt(1);
            packet.WriteVInt(4);

            packet.WriteHex(
                "7F020203007F7F0000000500000000007F7F7F7F7F7F7F7F0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");

            packet.WriteVInt(0);
            packet.WriteVInt(1400);

            packet.WriteVInt(0);
            packet.WriteVInt(560);

            packet.WriteVInt(0);
            packet.WriteVInt(1400);

            packet.WriteVInt(0);
            packet.WriteVInt(560);

            packet.WriteVInt(0);
            packet.WriteVInt(960);

            packet.WriteVInt(0);
            packet.WriteVInt(2400);

            for (var index = 0; index < towers; index++)
                packet.WriteHex("00000000000000A401A401");*/

            #endregion SectorState
        }

        /// <summary>
        ///     Stops the battle
        /// </summary>
        public void Stop()
        {
            if (!Resources.Configuration.UseUdp)
                BattleTimer.Stop();

            Resources.Battles.Remove(BattleId);
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
                foreach (var team in Teams.ToArray())
                foreach (var player in team)
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

                            player.Battle = null;

                            Remove(player);
                        }
                    }
                    else
                    {
                        Remove(player);
                    }

                if (Teams.FindIndex(t => t.FindIndex(p => p.Device.SecondsSinceLastCommand < 10) > -1) <= -1)
                    Stop();
            }
            catch (Exception)
            {
                Logger.Log("BattleTick failed.", GetType(), ErrorLevel.Error);
            }
        }

        /// <summary>
        ///     Remove a player from the battle and stop it when it's empty
        /// </summary>
        /// <param name="player"></param>
        public void Remove(Player player)
        {
            if (Teams.Count <= 0)
                Stop();

            foreach (var team in Teams.ToArray())
            {
                if (team.Contains(player)) team.Remove(player);

                if (team.Count <= 0) Teams.Remove(team);
            }
        }

        /*public Device GetEnemy(long userId)
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
        }*/
    }
}