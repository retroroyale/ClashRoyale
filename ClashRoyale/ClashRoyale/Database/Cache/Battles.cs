using System;
using System.Collections.Generic;
using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;

namespace ClashRoyale.Database.Cache
{
    public class Battles : Dictionary<long, Battle>
    {
        private long _seed = 1;

        public List<Player> PlayerQueue = new List<Player>();

        public Random Random = new Random();

        /// <summary>
        /// Get a player from the queue and remove it
        /// </summary>
        public Player Dequeue
        {
            get
            {
                Player player;

                lock (PlayerQueue)
                {
                    if (PlayerQueue.Count <= 0) return null;

                    player = PlayerQueue[0];
                    PlayerQueue.RemoveAt(0);

                    if (!player.Device.IsConnected)
                        return null;
                }

                return player;
            }
        }

        /// <summary>
        /// Adds a player to the queue and sends the estimated time
        /// </summary>
        /// <param name="player"></param>
        public void Enqueue(Player player)
        {
            lock (PlayerQueue)
            {
                if (PlayerQueue.Contains(player)) return;

                PlayerQueue.Add(player);

                var estimatedTime = Random.Next(601, 901);

                if (Count > 0)
                    if (Count > 5)
                        if (Count > 25)
                            estimatedTime = Count > Random.Next(61, 101) ? 5 : Random.Next(6, 16);
                        else
                            estimatedTime = Random.Next(30, 61);
                    else
                        estimatedTime = Random.Next(101, 601);

                SendInfo(player.Device, estimatedTime);
            }
        }

        /// <summary>
        /// Sends MatchmakeInfoMessage
        /// </summary>
        /// <param name="device"></param>
        /// <param name="estimatedDuration"></param>
        public async void SendInfo(Device device, int estimatedDuration)
        {
            await new MatchmakeInfoMessage(device)
            {
                EstimatedDuration = estimatedDuration
            }.Send();
        }

        /// <summary>
        /// Remove a player from queue and returns true wether he has been removed
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool Cancel(Player player)
        {
            lock (PlayerQueue)
            {
                if (!PlayerQueue.Contains(player)) return false;

                PlayerQueue.Remove(player);

                return true;
            }
        }

        /// <summary>
        /// Adds a battle to the list
        /// </summary>
        /// <param name="battle"></param>
        public void Add(Battle battle)
        {
            battle.BattleId = _seed++;

            if (!ContainsKey(battle.BattleId))
                Add(battle.BattleId, battle);
        }

        /// <summary>
        /// Remove a battle with the id
        /// </summary>
        /// <param name="id"></param>
        public new void Remove(long id)
        {
            if (ContainsKey(id))
                base.Remove(id);
        }
    }
}