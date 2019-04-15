using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class ResourcePacks : Data
    {
        public ResourcePacks(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }
        public string TID { get; set; }
        public string Resource { get; set; }
        public int Amount { get; set; }
        public string IconFile { get; set; }
    }
}