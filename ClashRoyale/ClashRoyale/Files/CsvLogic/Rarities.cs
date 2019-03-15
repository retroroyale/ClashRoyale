using System.Collections.Generic;
using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class Rarities : Data
    {
        public Rarities(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }

        public int LevelCount { get; set; }

        public int RelativeLevel { get; set; }

        public int MirrorRelativeLevel { get; set; }

        public int CloneRelativeLevel { get; set; }

        public int DonateCapacity { get; set; }

        public int SortCapacity { get; set; }

        public int DonateReward { get; set; }

        public int DonateXP { get; set; }

        public int GoldConversionValue { get; set; }

        public int ChanceWeight { get; set; }

        public int BalanceMultiplier { get; set; }

        public List<int> UpgradeExp { get; set; }

        public List<int> UpgradeMaterialCount { get; set; }

        public List<int> UpgradeCost { get; set; }

        public int PowerLevelMultiplier { get; set; }

        public int RefundGems { get; set; }

        public string TID { get; set; }

        public string CardBaseFileName { get; set; }

        public string BigFrameExportName { get; set; }

        public string CardBaseExportName { get; set; }

        public string StackedCardExportName { get; set; }

        public string CardRewardExportName { get; set; }

        public string CastEffect { get; set; }

        public string InfoTitleExportName { get; set; }

        public string CardRarityBGExportName { get; set; }

        public int SortOrder { get; set; }

        public int Red { get; set; }

        public int Green { get; set; }

        public int Blue { get; set; }

        public string AppearEffect { get; set; }

        public string BuySound { get; set; }

        public string LoopEffect { get; set; }

        public int CardTxtBgFrameIdx { get; set; }

        public string CardGlowInstanceName { get; set; }

        public string SpellSelectedSound { get; set; }

        public string SpellAvailableSound { get; set; }

        public string RotateExportName { get; set; }

        public string IconSWF { get; set; }

        public string IconExportName { get; set; }
    }
}