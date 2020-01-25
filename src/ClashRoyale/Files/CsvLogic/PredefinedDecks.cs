using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class PredefinedDecks : Data
    {
        public PredefinedDecks(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row, 12);
        }

        public string Name { get; set; }
        public string[] Spells { get; set; }
        public int SpellLevel { get; set; }
        public string RandomSpellSets { get; set; }
        public string Description { get; set; }
        public string TID { get; set; }
    }
}