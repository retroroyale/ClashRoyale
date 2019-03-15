using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class ContentTests : Data
    {
        public ContentTests(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }

        public string SourceData { get; set; }

        public string TargetData { get; set; }

        public string Stat1 { get; set; }

        public string Operator { get; set; }

        public string Stat2 { get; set; }

        public int Result { get; set; }

        public bool Enabled { get; set; }
    }
}