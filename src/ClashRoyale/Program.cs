using System;

namespace ClashRoyale
{
    public static class Program
    {
        public static string[] Arguments { get; private set; }

        private static void Main(string[] args)
        {
            Arguments = args ?? Array.Empty<string>();

            Console.Title = "ClashRoyale Server Emulator";

            Console.WriteLine(
                "\n______________             ______ ________                     ______     \r\n__  ____/__  /_____ __________  /____  __ \\__________  _______ ___  /____ \r\n_  /    __  /_  __ `/_  ___/_  __ \\_  /_/ /  __ \\_  / / /  __ `/_  /_  _ \\\r\n/ /___  _  / / /_/ /_(__  )_  / / /  _, _// /_/ /  /_/ // /_/ /_  / /  __/\r\n\\____/  /_/  \\__,_/ /____/ /_/ /_//_/ |_| \\____/_\\__, / \\__,_/ /_/  \\___/ \r\n                                                /____/                    \n\n");

            Resources.Initialize();

            Console.Read();
        }

        public static void Exit()
        {
            Environment.Exit(0);
        }
    }
}