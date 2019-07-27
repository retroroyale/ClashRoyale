using System;
using System.IO;
using System.Linq;
using ClashRoyale.CsvConverter.Extensions;

namespace ClashRoyale.CsvConverter
{
    public class Program
    {
        public static void Main()
        {
            Console.Title = "ClashRoyale CSV Converter v0.2";

            Console.SetOut(new Prefixed());

            Console.WriteLine("Converting...");

            if (!Directory.Exists("CSV Input"))
            {
                Directory.CreateDirectory("CSV Input");

                Console.WriteLine("Input Directory was not found so it has been created.");
            }

            if (!Directory.Exists("CS Output"))
            {
                Directory.CreateDirectory("CS Output");

                Console.WriteLine("Output Directory was not found so it has been created.");
            }

            var name = Console.ReadLine();

            if (Directory.GetFiles("CSV Input").Any())
            {
                var files = Directory.GetFiles("CSV Input");

                foreach (var file in files)
                {
                    if (Path.GetExtension(file) != ".csv") continue;

                    var header = File.ReadLines(file).ToList()[0].Replace("\"", "").Split(',');
                    var types = File.ReadLines(file).ToList()[1].Replace("\"", "").Split(',');

                    new CsWriter(Path.GetFileNameWithoutExtension(file), header, types, name);

                    Console.WriteLine($"File {Path.GetFileNameWithoutExtension(file)} has been exported.");
                }
            }
            else
            {
                Console.WriteLine("No CSV File has been found.");
            }

            Console.ReadKey();
        }
    }
}