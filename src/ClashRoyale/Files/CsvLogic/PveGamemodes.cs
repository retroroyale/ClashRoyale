using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class PveGamemodes : Data
    {
        public PveGamemodes(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row, 79);
        }

        public string Name { get; set; }
        public string Waves { get; set; }
        public string VictoryCondition { get; set; }
        public string ForcedCards { get; set; }
        public string Location { get; set; }
        public string ComputerPlayerType { get; set; }
        public string TowerRules { get; set; }
    }
}