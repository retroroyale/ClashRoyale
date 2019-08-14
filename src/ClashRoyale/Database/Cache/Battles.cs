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

                lock (_playerQueue)
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
        ///     Adds a player to the queue and sends the estimated time
        /// </summary>
        /// <param name="player"></param>
        public async void Enqueue(Player player)
        {
            var players = Resources.Players;
            var playerCount = players.Count;

            lock (_playerQueue)
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

            if (playerCount > 100) return;

            // Notify other players 
            foreach (var p in players.Values.ToList())
                if (p.Device.IsConnected && p.Home.Id != player.Home.Id)
                    await new PvpMatchmakeNotificationMessage(p.Device).SendAsync();
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
            lock (_playerQueue)
            {
                if (!_playerQueue.Contains(player)) return false;

                _playerQueue.Remove(player);

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

        /// <summary>
        ///     Get a battle by it's id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LogicBattle Get(long id)
        {
            return ContainsKey(id) ? this[id] : null;
        }
    }
}