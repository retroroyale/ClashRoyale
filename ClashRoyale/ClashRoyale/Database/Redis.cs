using System;
using System.Linq;
using System.Threading.Tasks;
using ClashRoyale.Logic;
using Newtonsoft.Json;
using SharpRaven.Data;
using StackExchange.Redis;

namespace ClashRoyale.Database
{
    public class Redis
    {
        private static IDatabase _players;
        private static IServer _server;

        private static ConnectionMultiplexer _connection;

        public static JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ObjectCreationHandling = ObjectCreationHandling.Reuse,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.None
        };

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
                _server = _connection.GetServer(Resources.Configuration.RedisServer, 6379);

                Logger.Log($"Successfully loaded Redis with {CachedPlayers()} player(s)", GetType());
            }
            catch (Exception exception)
            {
                Logger.Log(exception, GetType(), ErrorLevel.Error);
            }
        }

        public static bool IsConnected => _server != null;

        public static async Task CachePlayer(Player player)
        {
            try
            {
                await _players.StringSetAsync(player.Home.PlayerId.ToString(),
                    JsonConvert.SerializeObject(player, Settings), TimeSpan.FromHours(4));
            }
            catch (Exception exception)
            {
                Logger.Log(exception, null, ErrorLevel.Error);
            }
        }

        public static async Task UncachePlayer(long id)
        {
            try
            {
                await _players.KeyDeleteAsync(id.ToString());
            }
            catch (Exception exception)
            {
                Logger.Log(exception, null, ErrorLevel.Error);
            }
        }

        public static async Task<Player> GetCachedPlayer(long id)
        {
            try
            {
                var data = await _players.StringGetAsync(id.ToString());

                if (!string.IsNullOrEmpty(data)) return JsonConvert.DeserializeObject<Player>(data, Settings);

                var player = await PlayerDb.Get(id);
                await CachePlayer(player);
                return player;
            }
            catch (Exception exception)
            {
                Logger.Log(exception, null, ErrorLevel.Error);
            }

            return null;
        }

        public static int CachedPlayers()
        {
            try
            {
                return Convert.ToInt32(
                    _connection.GetServer(Resources.Configuration.RedisServer, 6379).Info("keyspace")[0]
                        .ElementAt(_players.Database)
                        .Value
                        .Split(new[] {"keys="}, StringSplitOptions.None)[1]
                        .Split(new[] {",expires="}, StringSplitOptions.None)[0]);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}