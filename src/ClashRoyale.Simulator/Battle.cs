using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using ClashRoyale.Simulator.Types;

namespace ClashRoyale.Simulator
{
    public class Battle
    {
        private readonly Timer _timer = new Timer(500)
        {
            AutoReset = true
        };

        private DateTime StartTime { get; set; }

        public List<TroopContext> Troops = new List<TroopContext>();
        public int Time => (int)DateTime.UtcNow.Subtract(StartTime).TotalSeconds * 2;
        public int BattleTick { get; set; }

        public Battle()
        {
            _timer.Elapsed += Tick;

            Troops.Add(TroopContext.Create(1, 3499, 23499));
        }

        public void Start()
        {
            StartTime = DateTime.UtcNow;
            _timer.Start();
        }

        public void Tick(object sender, ElapsedEventArgs args)
        {
            var st = new Stopwatch();
            st.Start();

            foreach (var ctx in Troops)
            {
                ctx.Tick();
            }

            st.Stop();

            BattleTick++;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Tick {BattleTick} done in {st.ElapsedMilliseconds}ms.");
            Console.ResetColor();
        }
    }
}
