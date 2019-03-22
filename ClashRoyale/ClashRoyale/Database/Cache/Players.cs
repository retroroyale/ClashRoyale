using System.Collections.Generic;
using System.Threading.Tasks;
using ClashRoyale.Logic;
using SharpRaven.Data;

namespace ClashRoyale.Database.Cache
{
    public class Players : Dictionary<long, Player>
    {
        public object SyncObject = new object();

        public void Login(Player player)
        {
            lock (SyncObject)
            {
                if (!ContainsKey(player.Home.Id)) Add(player.Home.Id, player);
            }
        }

        public void Logout(long userId)
        {
            lock (SyncObject)
            {
                if (ContainsKey(userId))
                {
                    var player = this[userId];

                    Resources.Battles.Cancel(player);

                    player.Save();

                    var result = Remove(userId);

                    if (!result) Logger.Log($"Couldn't logout player {userId}", GetType(), ErrorLevel.Error);
                }
            }
        }

        public async Task<Player> GetPlayer(long userId, bool onlineOnly = false)
        {
            lock (SyncObject)
            {
                if (ContainsKey(userId))
                    return this[userId];
            }

            if (onlineOnly) return null;

            if (!Redis.IsConnected) return await PlayerDb.Get(userId);

            var player = await Redis.GetPlayer(userId);

            if (player != null) return player;

            player = await PlayerDb.Get(userId);

            await Redis.Cache(player);

            return player;
        }
    }
}