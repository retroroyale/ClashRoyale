using System;

namespace ClashRoyale.Battles
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "ClashRoyale Battle Server Emulator";

            Console.WriteLine(
                "\n______________             ______ ________                     ______     \r\n__  ____/__  /_____ __________  /____  __ \\__________  _______ ___  /____ \r\n_  /    __  /_  __ `/_  ___/_  __ \\_  /_/ /  __ \\_  / / /  __ `/_  /_  _ \\\r\n/ /___  _  / / /_/ /_(__  )_  / / /  _, _// /_/ /  /_/ // /_/ /_  / /  __/\r\n\\____/  /_/  \\__,_/ /____/ /_/ /_//_/ |_| \\____/_\\__, / \\__,_/ /_/  \\___/ \r\n                                                /____/               Battles\n\n");

            Resources.Initialize();

            Console.Read();
        }
    }
}