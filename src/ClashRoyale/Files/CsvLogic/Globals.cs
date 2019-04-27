using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class Globals : Data
    {
        public Globals(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row, 3);
        }

        public string Name { get; set; }
        public int NumberValue { get; set; }
        public bool BooleanValue { get; set; }
        public string TextValue { get; set; }
        public string StringArray { get; set; }
        public int NumberArray { get; set; }
    }
}