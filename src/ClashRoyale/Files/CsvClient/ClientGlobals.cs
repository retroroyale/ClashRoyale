using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvClient
{
    public class ClientGlobals : Data
    {
        public ClientGlobals(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row, 20);
        }

        public string Name { get; set; }
        public int NumberValue { get; set; }
        public bool BooleanValue { get; set; }
        public string TextValue { get; set; }
        public string StringArray { get; set; }
        public int NumberArray { get; set; }
    }
}