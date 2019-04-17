using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class Achievements : Data
    {
        public Achievements(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row, 60);
        }

        public string Name { get; set; }
        public int Level { get; set; }
        public string TID { get; set; }
        public string InfoTID { get; set; }
        public string Action { get; set; }
        public int ActionCount { get; set; }
        public int ExpReward { get; set; }
        public int DiamondReward { get; set; }
        public int SortIndex { get; set; }
        public bool Hidden { get; set; }
        public string AndroidID { get; set; }
        public string Type { get; set; }
    }
}