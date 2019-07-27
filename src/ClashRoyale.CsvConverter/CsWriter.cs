using System.Collections.Generic;
using System.IO;

namespace ClashRoyale.CsvConverter
{
    public class CsWriter
    {
        internal CsWriter(string name, IReadOnlyList<string> header, string[] types, string space)
        {
            using (var writer = new StreamWriter($"CS Output/{Uppercase(name)}.cs"))
            {
                writer.WriteLine($"namespace ClashRoyale.Files.{space}");
                writer.WriteLine("{");
                writer.WriteLine($"    public class {Uppercase(name)} : Data");
                writer.WriteLine("    {");
                writer.WriteLine(
                    $"        public {Uppercase(name)}(Row row, DataTable datatable) : base(row, datatable)");
                writer.WriteLine("        {");
                writer.WriteLine("            LoadData(this, GetType(), row);");
                writer.WriteLine("        }");
                writer.WriteLine();

                var count = header.Count;

                for (var index = 0; index < count; index++)
                {
                    var type = types[index].ToLower() == "boolean" ? "bool" : types[index].ToLower();

                    writer.WriteLine("        public " + type + " " + header[index] + " { get; set; }");

                    if (index < count - 1)
                        writer.WriteLine();
                }

                writer.WriteLine("    }");
                writer.WriteLine("}");
            }
        }

        internal string Uppercase(string _string)
        {
            if (string.IsNullOrEmpty(_string))
                return string.Empty;

            var result = _string.Split('_');
            var newString = string.Empty;
            foreach (var s in result)
            {
                var _char = s.ToCharArray();
                _char[0] = char.ToUpper(_char[0]);

                newString += new string(_char);
            }

            return newString;
        }
    }
}