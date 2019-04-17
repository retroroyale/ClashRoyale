using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvClient
{
    public class BackgroundDecos : Data
    {
        public BackgroundDecos(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row, 50);
        }

        public string Name { get; set; }
        public string FileName { get; set; }
        public string ExportName { get; set; }
        public string Layer { get; set; }
    }
}