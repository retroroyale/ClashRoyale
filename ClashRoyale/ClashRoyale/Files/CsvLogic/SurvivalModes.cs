using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class SurvivalModes : Data
    {
        public SurvivalModes(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }

        public string IconSWF { get; set; }

        public string IconExportName { get; set; }

        public string GameMode { get; set; }

        public string WinsIconExportName { get; set; }

        public bool Enabled { get; set; }

        public bool EventOnly { get; set; }

        public int JoinCost { get; set; }

        public string JoinCostResource { get; set; }

        public int FreePass { get; set; }

        public int MaxWins { get; set; }

        public int MaxLoss { get; set; }

        public int RewardCards { get; set; }

        public int RewardGold { get; set; }

        public int RewardSpellCount { get; set; }

        public string RewardSpell { get; set; }

        public int RewardSpellMaxCount { get; set; }

        public string ItemExportName { get; set; }

        public string ConfirmExportName { get; set; }

        public string TID { get; set; }

        public string CardTheme { get; set; }
    }
}