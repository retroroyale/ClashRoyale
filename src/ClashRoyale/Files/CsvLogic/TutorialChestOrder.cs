using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class TutorialChestOrder : Data
    {
        public TutorialChestOrder(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row, 82);
        }

        public string Name { get; set; }
        public string Chest { get; set; }
        public string NPC { get; set; }
        public string PvE_Tutorial { get; set; }
    }
}