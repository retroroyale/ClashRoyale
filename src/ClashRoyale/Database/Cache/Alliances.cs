using System.Collections.Generic;
using System.Threading.Tasks;
using ClashRoyale.Logic.Clan;
using SharpRaven.Data;

namespace ClashRoyale.Database.Cache
{
    public class Alliances : Dictionary<long, Alliance>
    {
        private readonly object _syncObject = new object();

        /// <summary>
        ///     Add an alliance to the server
        /// </summary>
        /// <param name="alliance"></param>
        public void Add(Alliance alliance)
        {
            lock (_syncObject)
            {
                if (!ContainsKey(alliance.Id)) Add(alliance.Id, alliance);
            }
        }

        /// <summary>
        ///     Remove an alliance from the server and save it
        /// </summary>
        /// <param name="allianceId"></param>
        public new void Remove(long allianceId)
        {
            lock (_syncObject)
            {
                if (ContainsKey(allianceId))
                {
                    var alliance = this[allianceId];

                    alliance.Save();

                    var result = base.Remove(allianceId);

                    if (!result) Logger.Log($"Couldn't remove alliance {allianceId}", GetType(), ErrorLevel.Error);
                }
            }
        }

        /// <summary>
        ///     Get an alliance from cache or database
        /// </summary>
        /// <param name="allianceId"></param>
        /// <param name="onlineOnly"></param>
        /// <returns></returns>
        public async Task<Alliance> GetAllianceAsync(long allianceId, bool onlineOnly = false)
        {
            lock (_syncObject)
            {
                if (ContainsKey(allianceId))
                    return this[allianceId];
            }

            if (onlineOnly) return null;

            var alliance = Resources.ObjectCache.GetCachedAlliance(allianceId);

            if (alliance != null) return alliance;

            alliance = await AllianceDb.GetAsync(allianceId);

            Resources.ObjectCache.CacheAlliance(alliance);

            return alliance;
        }
    }
}