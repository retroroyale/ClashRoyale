using System.Collections.Generic;
using System.Threading.Tasks;
using ClashRoyale.Logic;
using SharpRaven.Data;

namespace ClashRoyale.Database.Cache
{
    public class Players : Dictionary<long, Player>
    {
        private readonly object _syncObject = new object();

        /// <summary>
        /// Login a player
        /// </summary>
        /// <param name="player"></param>
        public void Login(Player player)
        {
            lock (_syncObject)
            {
                if (!ContainsKey(player.Home.Id)) Add(player.Home.Id, player);
            }
        }

        /// <summary>
        /// Logout a player and save it
        /// </summary>
        /// <param name="userId"></param>
        public void Logout(long userId)
        {
            lock (_syncObject)
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

        /// <summary>
        /// Get a player from cache or database
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="onlineOnly"></param>
        /// <returns></returns>
        public async Task<Player> GetPlayerAsync(long userId, bool onlineOnly = false)
        {
            lock (_syncObject)
            {
                if (ContainsKey(userId))
                    return this[userId];
            }

            if (onlineOnly) return null;

            if (!Redis.IsConnected) return await PlayerDb.GetAsync(userId);

            var player = await Redis.GetPlayerAsync(userId);

            if (player != null) return player;

            player = await PlayerDb.GetAsync(userId);

            await Redis.CacheAsync(player);

            return player;
        }
    }
}