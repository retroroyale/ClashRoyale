using System;

namespace ClashRoyale
{
    public class Program
    {
        private static void Main()
        {
            Console.Title = "ClashRoyale";

            Resources.Initialize();

            Console.ReadKey(true);
        }
    }
}