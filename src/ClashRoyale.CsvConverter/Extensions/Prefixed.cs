using System;
using System.IO;
using System.Text;

namespace ClashRoyale.CsvConverter.Extensions
{
    internal class Prefixed : TextWriter
    {
        public readonly TextWriter Original;

        public Prefixed()
        {
            Original = Console.Out;
        }

        public override Encoding Encoding => new ASCIIEncoding();

        public override void Write(string text)
        {
            Original.Write(text);
        }

        public override void WriteLine(string text)
        {
            Original.WriteLine($"[{DateTime.Now.ToLongTimeString()}]    {text}");
        }

        public override void Write(char text)
        {
            Original.WriteLine($"[{DateTime.Now.ToLongTimeString()}]    {text}");
        }

        public override void WriteLine()
        {
            Original.WriteLine();
        }
    }
}