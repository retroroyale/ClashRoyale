using System;
using System.Collections.Generic;
using System.Linq;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Battle;
using ClashRoyale.Protocol.Messages.Server;

namespace ClashRoyale.Database.Cache
{
    public class Battles : Dictionary<long, LogicBattle>
    {
        private readonly List<Player> _playerQueue = new List<Player>();
        private readonly List<Player> _duoPlayerQueue = new List<Player>();

        private readonly object _playerQueueSync = new object();
        private readonly object _duoPlayerQueueSync = new object();

        private readonly Random _random = new Random();
        private long _seed = 1;

        /// <summary>
        ///     Get a player from the queue and remove it
        /// </summary>
        public Player Dequeue
        {
            get
            {
                Player player;

                lock (_playerQueueSync)
                {
                    if (_playerQueue.Count <= 0) return null;

                    player = _playerQueue[0];
                    _playerQueue.RemoveAt(0);

                    if (!player.Device.IsConnected)
                        return null;
                }

                return player;
            }
        }

        /// <summary>
        ///     Get all 4 players from the duo queue and remove them
        /// </summary>
        public List<Player> DequeueDuo
        {
            get
            {
                /*Player player;

                lock (_duoPlayerQueueSync)
                {
                    if (_duoPlayerQueue.Count <= 0) return null;

                    player = _duoPlayerQueue[0];
                    _duoPlayerQueue.RemoveAt(0);

                    if (!player.Device.IsConnected)
                        return null;
                }

                return player;*/

                return null;
            }
        }

        /// <summary>
        ///     Adds a player to the queue and sends the estimated time
        /// </summary>
        /// <param name="player"></param>
        /// <param name="duo"></param>
        public async void Enqueue(Player player, bool duo)
        {
            var players = Resources.Players;
            var playerCount = players.Count;

            if (!duo)
            {
                lock (_playerQueueSync)
                {
                    if (_playerQueue.Contains(player)) return;
                    _playerQueue.Add(player);

                    var estimatedTime = _random.Next(601, 901);

                    if (playerCount > 0)
                        if (playerCount > 5)
                            if (playerCount > 25)
                                estimatedTime = playerCount > _random.Next(61, 101) ? 5 : _random.Next(6, 16);
                            else
                                estimatedTime = _random.Next(30, 61);
                        else
                            estimatedTime = _random.Next(101, 601);

                    SendInfo(player.Device, estimatedTime);
                }
            }
            else
            {
                lock (_duoPlayerQueueSync)
                {
                    if (_duoPlayerQueue.Contains(player)) return;
                    _duoPlayerQueue.Add(player);
                }

                // TODO SEND INFO
            }

            if (playerCount > 100) return;

            // Notify other players 
            foreach (var p in players.Values.ToList())
            {
                if (p.Device.IsConnected && p.Home.Id != player.Home.Id)
                {
                    await new PvpMatchmakeNotificationMessage(p.Device).SendAsync();
                }
            }
        }

        /// <summary>
        ///     Sends MatchmakeInfoMessage
        /// </summary>
        /// <param name="device"></param>
        /// <param name="estimatedDuration"></param>
        public async void SendInfo(Device device, int estimatedDuration)
        {
            await new MatchmakeInfoMessage(device)
            {
                EstimatedDuration = estimatedDuration
            }.SendAsync();
        }

        /// <summary>
        ///     Remove a player from queue and returns true wether he has been removed
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool Cancel(Player player)
        {
            lock (_playerQueueSync)
            {
                if (_playerQueue.Contains(player))
                {
                    _playerQueue.Remove(player);

                    return true;
                }
            }

            lock (_duoPlayerQueueSync)
            {
                if (!_duoPlayerQueue.Contains(player)) return false;

                _duoPlayerQueue.Remove(player);

                return true;
            }
        }

        /// <summary>
        ///     Adds a battle to the list
        /// </summary>
        /// <param name="battle"></param>
        public void Add(LogicBattle battle)
        {
            battle.BattleId = _seed++;

            if (!ContainsKey(battle.BattleId))
                Add(battle.BattleId, battle);
        }

        /// <summary>
        ///     Remove a battle with the id
        /// </summary>
        /// <param name="id"></param>
        public new void Remove(long id)
        {
            if (ContainsKey(id))
                base.Remove(id);
        }
    }
}
