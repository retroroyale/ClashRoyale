using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClashRoyale.Core;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Clan;
using ClashRoyale.Logic.Sessions;
using Newtonsoft.Json;
using SharpRaven.Data;
using StackExchange.Redis;

namespace ClashRoyale.Database
{
    public class Redis
    {
        private static IDatabase _players;
        private static IDatabase _sessions;
        private static IDatabase _alliances;
        private static IServer _server;

        private static ConnectionMultiplexer _connection;

        public Redis()
        {
            try
            {
                var config = new ConfigurationOptions
                {
                    AllowAdmin = true,
                    ConnectTimeout = 10000,
                    ConnectRetry = 10,
                    HighPrioritySocketThreads = true,
                    Password = Resources.Configuration.RedisPassword
                };

                config.EndPoints.Add(Resources.Configuration.RedisServer, 6379);

                _connection = ConnectionMultiplexer.Connect(config);

                _players = _connection.GetDatabase(0);
                _alliances = _connection.GetDatabase(1);
                _sessions = _connection.GetDatabase(2);
                _server = _connection.GetServer(Resources.Configuration.RedisServer, 6379);

                Logger.Log(
                    _server.IsConnected
                        ? $"Successfully loaded Redis with {CachedPlayers()} player(s) & {CachedAlliances()} clan(s)"
                        : $"RedisConnection {Resources.Configuration.RedisServer} failed!", GetType());
            }
            catch (Exception exception)
            {
                Logger.Log(exception, GetType(), ErrorLevel.Error);
            }
        }

        /// <summary>
        ///     Returns true wether the client is connected
        /// </summary>
        public static bool IsConnected => _server != null;

        /// <summary>
        ///     Cache a player
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static async Task CacheAsync(Player player)
        {
            if (player == null) return;

            try
            {
                await _players.StringSetAsync(player.Home.Id.ToString(),
                    JsonConvert.SerializeObject(player, Configuration.JsonSettings), TimeSpan.FromHours(4));

                await _sessions.StringSetAsync(player.Home.Id.ToString(),
                    JsonConvert.SerializeObject(player.Home.Sessions, Configuration.JsonSettings),
                    TimeSpan.FromHours(4));
            }
            catch (Exception exception)
            {
                Logger.Log(exception, null, ErrorLevel.Error);
            }
        }

        /// <summary>
        ///     Cache an alliance
        /// </summary>
        /// <param name="alliance"></param>
        /// <returns></returns>
        public static async Task CacheAsync(Alliance alliance)
        {
            if (alliance == null) return;

            try
            {
                await _alliances.StringSetAsync(alliance.Id.ToString(),
                    JsonConvert.SerializeObject(alliance, Configuration.JsonSettings), TimeSpan.FromHours(4));
            }
            catch (Exception exception)
            {
                Logger.Log(exception, null, ErrorLevel.Error);
            }
        }

        /// <summary>
        ///     Uncache a player
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task UncachePlayerAsync(long id)
        {
            try
            {
                await _players.KeyDeleteAsync(id.ToString());
                await _sessions.KeyDeleteAsync(id.ToString());
            }
            catch (Exception exception)
            {
                Logger.Log(exception, null, ErrorLevel.Error);
            }
        }

        /// <summary>
        ///     Uncache an alliance
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task UncacheAllianceAsync(long id)
        {
            try
            {
                await _alliances.KeyDeleteAsync(id.ToString());
            }
            catch (Exception exception)
            {
                Logger.Log(exception, null, ErrorLevel.Error);
            }
        }

        /// <summary>
        ///     Get the player from the cache
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<Player> GetPlayerAsync(long id)
        {
            try
            {
                var data = await _players.StringGetAsync(id.ToString());
                var sessions = await _sessions.StringGetAsync(id.ToString());

                if (!string.IsNullOrEmpty(data) && !string.IsNullOrEmpty(sessions))
                {
                    var player = JsonConvert.DeserializeObject<Player>(data, Configuration.JsonSettings);
                    player.Home.Sessions =
                        JsonConvert.DeserializeObject<List<Session>>(sessions, Configuration.JsonSettings) ??
                        new List<Session>(50);
                    return player;
                }
            }
            catch (Exception exception)
            {
                Logger.Log(exception, null, ErrorLevel.Error);
            }

            return null;
        }

        /// <summary>
        ///     Get an alliance from the cache
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<Alliance> GetAllianceAsync(long id)
        {
            try
            {
                var data = await _alliances.StringGetAsync(id.ToString());

                if (!string.IsNullOrEmpty(data))
                    return JsonConvert.DeserializeObject<Alliance>(data, Configuration.JsonSettings);

                var alliance = await AllianceDb.GetAsync(id);
                await CacheAsync(alliance);
                return alliance;
            }
            catch (Exception exception)
            {
                Logger.Log(exception, null, ErrorLevel.Error);
            }

            return null;
        }

        /// <summary>
        ///     Get a random alliance from the cache
        /// </summary>
        /// <returns></returns>
        public static async Task<Alliance> GetRandomAllianceAsync()
        {
            try
            {
                return await GetAllianceAsync(long.Parse(await _alliances.KeyRandomAsync()));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Returns the amount of cached players
        /// </summary>
        /// <returns></returns>
        public static int CachedPlayers()
        {
            try
            {
                var keyspace = _connection.GetServer(Resources.Configuration.RedisServer, 6379).Info("keyspace")[0];

                return Convert.ToInt32(
                    keyspace.FirstOrDefault(x => x.Key.Replace("db", string.Empty) == _players.Database.ToString())
                        .Value
                        .Split(new[] {"keys="}, StringSplitOptions.None)[1]
                        .Split(new[] {",expires="}, StringSplitOptions.None)[0]);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        ///     Returns the amount of cached alliances
        /// </summary>
        /// <returns></returns>
        public static int CachedAlliances()
        {
            try
            {
                var keyspace = _connection.GetServer(Resources.Configuration.RedisServer, 6379).Info("keyspace")[0];

                return Convert.ToInt32(
                    keyspace.FirstOrDefault(x => x.Key.Replace("db", string.Empty) == _alliances.Database.ToString())
                        .Value
                        .Split(new[] { "keys=" }, StringSplitOptions.None)[1]
                        .Split(new[] { ",expires=" }, StringSplitOptions.None)[0]);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}