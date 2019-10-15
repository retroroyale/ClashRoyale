using System;

namespace ClashRoyale.Simulator
{
    public class Program
    {
        /// <summary>
        /// Test for battle simulation
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Console.Title = "ClashRoyale Battle Simulator [EXPERIMENTAL]";

            Console.ReadKey(true);

            Console.WriteLine("Battle starting...");

            var battle = new Battle();
            battle.Start();

            Console.Read();
        }
    }
}
