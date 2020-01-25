using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Clan;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using SharpRaven.Data;

namespace ClashRoyale.Database
{
    public class ObjectCache
    {
        private readonly TimeSpan _expirationTimeSpan;
        private readonly MemoryCache _playerCache;
        private readonly MemoryCache _allianceCache;

        public ObjectCache()
        {
            var options = new MemoryCacheOptions
            {
                ExpirationScanFrequency = TimeSpan.FromMinutes(5)
            };

            _expirationTimeSpan = TimeSpan.FromHours(2);

            _playerCache = new MemoryCache(options, new NullLoggerFactory());
            _allianceCache = new MemoryCache(options, new NullLoggerFactory());

            Logger.Log("Successfully loaded caches", null);
        }

        /// <summary>
        ///     Cache a player to access it from memory
        /// </summary>
        /// <param name="player"></param>
        public void CachePlayer(Player player)
        {
            try
            {
                var playerEntry = _playerCache.CreateEntry(player.Home.Id);
                playerEntry.Value = player;
                _playerCache.Set(player.Home.Id, playerEntry, _expirationTimeSpan);
            }
            catch (Exception)
            {
                Logger.Log("Failed to cache player.", GetType(), ErrorLevel.Error);
            }
        }

        /// <summary>
        ///     Cache a clan to access it from memory
        /// </summary>
        /// <param name="alliance"></param>
        public void CacheAlliance(Alliance alliance)
        {
            try
            {
                var allianceEntry = _allianceCache.CreateEntry(alliance.Id);
                allianceEntry.Value = alliance;
                _allianceCache.Set(alliance.Id, allianceEntry, _expirationTimeSpan);
            }
            catch (Exception)
            {
                Logger.Log("Failed to cache player.", GetType(), ErrorLevel.Error);
            }
        }

        public Player GetCachedPlayer(long id)
        {
            try
            {
                var st = new Stopwatch();
                st.Start();

                if (_playerCache.Get(id) is ICacheEntry playerEntry)
                {
                    if (playerEntry.Value is Player player)
                    {
                        st.Stop();
                        Logger.Log($"Successfully got player {id} from cache in {st.ElapsedMilliseconds}ms", null,
                            ErrorLevel.Debug);

                        return player;
                    }
                }
            }
            catch (Exception)
            {
                Logger.Log("Failed to fetch player from cache.", GetType(), ErrorLevel.Error);
            }

            return null;
        }

        public Alliance GetCachedAlliance(long id)
        {
            try
            {
                var st = new Stopwatch();
                st.Start();

                if (_allianceCache.Get(id) is ICacheEntry allianceEntry)
                {
                    if (allianceEntry.Value is Alliance alliance)
                    {
                        st.Stop();
                        Logger.Log($"Successfully got alliance {id} from cache in {st.ElapsedMilliseconds}ms", null,
                            ErrorLevel.Debug);

                        return alliance;
                    }
                }
            }
            catch (Exception)
            {
                Logger.Log("Failed to fetch alliance from cache.", GetType(), ErrorLevel.Error);
            }

            return null;
        }

        public void UncacheAlliance(long id)
        {
            _allianceCache.Remove(id);
        }

        public long CachedPlayers()
        {
            return _playerCache.Count;
        }

        public long CachedClans()
        {
            return _allianceCache.Count;
        }
    }
}