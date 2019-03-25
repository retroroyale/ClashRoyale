using System.Collections.Generic;
using System.Threading.Tasks;
using SharpRaven.Data;
using ClashRoyale.Logic.Clan;

namespace ClashRoyale.Database.Cache
{
    public class Alliances : Dictionary<long, Alliance>
    {
        public object SyncObject = new object();

        public void Add(Alliance alliance)
        {
            lock (SyncObject)
            {
                if (!ContainsKey(alliance.Id)) Add(alliance.Id, alliance);
            }
        }

        public new void Remove(long allianceId)
        {
            lock (SyncObject)
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

        public async Task<Alliance> GetAlliance(long allianceId, bool onlineOnly = false)
        {
            lock (SyncObject)
            {
                if (ContainsKey(allianceId))
                    return this[allianceId];
            }

            if (onlineOnly) return null;

            if (!Redis.IsConnected) return await AllianceDb.Get(allianceId);

            var alliance = await Redis.GetAlliance(allianceId);

            if (alliance != null) return alliance;

            alliance = await AllianceDb.Get(allianceId);

            await Redis.Cache(alliance);

            return alliance;
        }
    }
}