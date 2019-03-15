using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClashRoyale.Logic;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using SharpRaven.Data;

namespace ClashRoyale.Database
{
    public class PlayerDb
    {
        private static string _connectionString;
        private static long _playerSeed;

        public static JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ObjectCreationHandling = ObjectCreationHandling.Reuse,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.None
        };

        public PlayerDb()
        {
            _connectionString = new MySqlConnectionStringBuilder
            {
                Server = Resources.Configuration.MySqlServer,
                Database = Resources.Configuration.MySqlDatabase,
                UserID = Resources.Configuration.MySqlUserId,
                Password = Resources.Configuration.MySqlPassword,
                SslMode = MySqlSslMode.None,
                MinimumPoolSize = 4,
                MaximumPoolSize = 20
            }.ToString();

            _playerSeed = MaxPlayerId();

            Logger.Log($"Successfully loaded MySql with {_playerSeed} player(s)", GetType());
        }

        public static async Task ExecuteAsync(MySqlCommand cmd)
        {
            try
            {
                cmd.Connection = new MySqlConnection(_connectionString);
                await cmd.Connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (MySqlException exception)
            {
                Logger.Log(exception, null, ErrorLevel.Error);
            }
            finally
            {
                cmd.Connection?.Close();
            }
        }

        public static long MaxPlayerId()
        {
            #region MaxPlayerId

            try
            {
                long seed;

                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var cmd = new MySqlCommand("SELECT coalesce(MAX(Id), 0) FROM player", connection))
                    {
                        seed = Convert.ToInt64(cmd.ExecuteScalar());
                    }

                    connection.Close();
                }

                return seed;
            }
            catch (Exception exception)
            {
                Logger.Log(exception, null, ErrorLevel.Fatal);

                return -1;
            }

            #endregion
        }

        public static async Task<long> PlayerCount()
        {
            #region PlayerCount

            try
            {
                long seed;

                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM player", connection))
                    {
                        seed = Convert.ToInt64(await cmd.ExecuteScalarAsync());
                    }

                    await connection.CloseAsync();
                }

                return seed;
            }
            catch (Exception exception)
            {
                Logger.Log(exception, null, ErrorLevel.Error);

                return 0;
            }

            #endregion
        }

        public static async Task<Player> Create()
        {
            try
            {
                var id = _playerSeed++;
                if (id <= -1)
                    return null;

                var player = new Player(id + 1);

                using (var cmd =
                    new MySqlCommand(
                        $"INSERT INTO player (`Id`, `Trophies`, `Language`, `Home`) VALUES ({id + 1}, {player.Home.Trophies}, @language, @home)")
                )
                {
#pragma warning disable 618
                    cmd.Parameters?.Add("@language", player.Home.PreferredDeviceLanguage);
                    cmd.Parameters?.Add("@home", JsonConvert.SerializeObject(player, Settings));
#pragma warning restore 618

                    await ExecuteAsync(cmd);
                }

                return player;
            }
            catch (Exception exception)
            {
                Logger.Log(exception, null, ErrorLevel.Error);

                return null;
            }
        }

        public static async Task<Player> Get(long id)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    Player player = null;

                    using (var cmd = new MySqlCommand($"SELECT * FROM player WHERE Id = '{id}'", connection))
                    {
                        var reader = await cmd.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                        {
                            player = JsonConvert.DeserializeObject<Player>((string) reader["Home"], Settings);
                            break;
                        }
                    }

                    await connection.CloseAsync();

                    return player;
                }
            }
            catch (Exception exception)
            {
                Logger.Log(exception, null, ErrorLevel.Fatal);

                return null;
            }
        }

        public static async Task<Player> Get(string facebookId)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    Player player = null;

                    using (var cmd = new MySqlCommand($"SELECT * FROM player WHERE FacebookId = '{facebookId}'",
                        connection))
                    {
                        var reader = await cmd.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                            player = JsonConvert.DeserializeObject<Player>((string) reader["Home"], Settings);
                    }

                    await connection.CloseAsync();

                    return player;
                }
            }
            catch (Exception exception)
            {
                Logger.Log(exception, null, ErrorLevel.Fatal);

                return null;
            }
        }

        public static async Task Save(Player player)
        {
            try
            {
                using (var cmd =
                    new MySqlCommand(
                        $"UPDATE player SET `Trophies`='{player.Home.Trophies}', `Language`='{player.Home.PreferredDeviceLanguage}', `FacebookId`=@fb, `Home`=@home WHERE Id = '{player.Home.PlayerId}'")
                )
                {
#pragma warning disable 618
                    cmd.Parameters?.Add("@fb", player.Home.FacebookId);
                    cmd.Parameters?.Add("@home", JsonConvert.SerializeObject(player, Settings));
#pragma warning restore 618

                    await ExecuteAsync(cmd);
                }
            }
            catch (Exception exception)
            {
                Logger.Log(exception, null, ErrorLevel.Error);
            }
        }

        public static async Task Delete(long id)
        {
            try
            {
                using (var cmd = new MySqlCommand(
                    $"DELETE FROM player WHERE Id = '{id}'")
                )
                {
                    await ExecuteAsync(cmd);

                    if (Redis.IsConnected)
                        await Redis.UncachePlayer(id);
                }
            }
            catch (Exception exception)
            {
                Logger.Log(exception, null, ErrorLevel.Error);
            }
        }

        public static async Task<List<Player>> GetGlobalPlayerRanking()
        {
            var list = new List<Player>();

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var cmd = new MySqlCommand("SELECT * FROM player ORDER BY `Trophies` DESC LIMIT 200",
                        connection))
                    {
                        var reader = await cmd.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                            list.Add(JsonConvert.DeserializeObject<Player>((string) reader["Home"], Settings));
                    }

                    await connection.CloseAsync();
                }

                return list;
            }
            catch (Exception exception)
            {
                Logger.Log(exception, null, ErrorLevel.Error);

                return list;
            }
        }

        public static async Task<List<Player>> GetLocalPlayerRanking(string language)
        {
            var list = new List<Player>();

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var cmd =
                        new MySqlCommand(
                            $"SELECT * FROM player WHERE Language = '{language}' ORDER BY `Trophies` DESC LIMIT 200",
                            connection))
                    {
                        var reader = await cmd.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                            list.Add(JsonConvert.DeserializeObject<Player>((string) reader["Home"], Settings));
                    }

                    await connection.CloseAsync();
                }

                return list;
            }
            catch (Exception exception)
            {
                Logger.Log(exception, null, ErrorLevel.Error);

                return list;
            }
        }
    }
}