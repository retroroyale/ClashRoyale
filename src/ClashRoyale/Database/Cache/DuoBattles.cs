using System.Collections.Generic;
using System.Linq;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Battle;
using ClashRoyale.Protocol.Messages.Server;

namespace ClashRoyale.Database.Cache
{
    public class DuoBattles : Dictionary<long, LogicBattle>
    {
        private readonly List<Player> _duoPlayerQueue = new List<Player>();

        private long _seed = 1;

        /// <summary>
        ///     Get 3 players from the duo queue and remove them
        /// </summary>
        public List<Player> Dequeue
        {
            get
            {
                lock (_duoPlayerQueue)
                {
                    if (_duoPlayerQueue.Count < 3) return null;

                    var players = new List<Player>();

                    for (var i = 0; i < 3; i++)
                    {
                        var player = _duoPlayerQueue[0];
                        _duoPlayerQueue.RemoveAt(0);

                        players.Add(player);
                    }

                    return players;
                }
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

            lock (_duoPlayerQueue)
            {
                if (_duoPlayerQueue.Contains(player)) return;

                _duoPlayerQueue.Add(player);
            }

            // TODO SEND INFO TO PLAYERS IN QUEUE

            if (playerCount > 100) return;

            // Notify other players 
            foreach (var p in players.Values.ToList())
                if (p.Device.IsConnected && p.Home.Id != player.Home.Id)
                    await new PvpMatchmakeNotificationMessage(p.Device).SendAsync();
        }

        /// <summary>
        ///     Remove a player from queue and returns true wether he has been removed
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool Cancel(Player player)
        {
            lock (_duoPlayerQueue)
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