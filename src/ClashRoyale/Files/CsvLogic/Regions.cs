using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class Regions : Data
    {
        public Regions(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row, 57);
        }

        public string Name { get; set; }
        public string TID { get; set; }
        public string DisplayName { get; set; }
        public bool IsCountry { get; set; }
        public bool RegionPopup { get; set; }
    }
}