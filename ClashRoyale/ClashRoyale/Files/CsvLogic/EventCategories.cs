using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class EventCategories : Data
    {
        public EventCategories(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }

        public string CSVFiles { get; set; }

        public string CSVRows { get; set; }

        public string CustomNames { get; set; }
    }
}