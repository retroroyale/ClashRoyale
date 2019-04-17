using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvClient
{
    public class Helpshift : Data
    {
        public Helpshift(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row, 62);
        }

        public string Name { get; set; }
        public string HelpshiftId { get; set; }
    }
}