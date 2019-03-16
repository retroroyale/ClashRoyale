using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using ClashRoyale.Database;
using ClashRoyale.Extensions;
using ClashRoyale.Files;
using ClashRoyale.Files.CsvLogic;
using ClashRoyale.Logic;
using SharpRaven.Data;

namespace ClashRoyale.Core.Leaderboards
{
    public class Leaderboard
    {
        private readonly Timer _timer = new Timer(20000)
        {
            AutoReset = true
        };

        public List<Player> GlobalPlayers = new List<Player>(200);

        public Dictionary<string, List<Player>> LocalPlayers = new Dictionary<string, List<Player>>(11);

        public Leaderboard()
        {
            _timer.Elapsed += TimerCallback;
            _timer.Start();

            foreach (var locales in Csv.Tables.Get(Csv.Types.Locales).GetDatas())
                LocalPlayers.Add(((Locales) locales).Name, new List<Player>(200));
        }

        public async void TimerCallback(object state, ElapsedEventArgs args)
        {
            await Task.Run(async () =>
            {
                try
                {
                    var currentGlobalPlayerRanking = await PlayerDb.GetGlobalPlayerRanking();
                    for (var i = 0; i < currentGlobalPlayerRanking.Count; i++)
                        GlobalPlayers.UpdateOrInsert(i, currentGlobalPlayerRanking[i]);
                }
                catch (Exception exception)
                {
                    Logger.Log($"Error while updating leaderboads {exception}", GetType(), ErrorLevel.Error);
                }
            });
        }
    }
}