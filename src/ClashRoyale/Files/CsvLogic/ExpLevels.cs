using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class ExpLevels : Data
    {
        public ExpLevels(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row, 46);
        }

        public string Name { get; set; }
        public int ExpToNextLevel { get; set; }
        public int SummonerLevel { get; set; }
        public int TowerLevel { get; set; }
        public int TroopLevel { get; set; }
        public int Decks { get; set; }
        public int SummonerKillGold { get; set; }
        public int TowerKillGold { get; set; }
        public int DiamondReward { get; set; }
    }
}