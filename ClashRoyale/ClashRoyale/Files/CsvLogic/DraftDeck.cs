using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class DraftDeck : Data
    {
        public DraftDeck(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }
        public string RequiredSets { get; set; }
        public string OptionalSets { get; set; }
    }
}