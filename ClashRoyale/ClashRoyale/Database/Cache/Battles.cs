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

        public async void SendInfo(Device device, int estimatedDuration)
        {
            await new MatchmakeInfoMessage(device)
            {
                EstimatedDuration = estimatedDuration
            }.Send();
        }

        public bool Cancel(Player player)
        {
            lock (PlayerQueue)
            {
                if (!PlayerQueue.Contains(player)) return false;

                PlayerQueue.Remove(player);

                return true;
            }
        }

        public void Add(Battle battle)
        {
            battle.BattleId = _seed++;

            if (!ContainsKey(battle.BattleId))
                Add(battle.BattleId, battle);
        }

        public new void Remove(long id)
        {
            if (ContainsKey(id))
                base.Remove(id);
        }
    }
}