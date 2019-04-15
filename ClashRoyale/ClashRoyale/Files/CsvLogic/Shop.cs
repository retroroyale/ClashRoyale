using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class Shop : Data
    {
        public Shop(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }
        public string Category { get; set; }
        public string TID { get; set; }
        public string Rarity { get; set; }
        public bool Disabled { get; set; }
        public string Resource { get; set; }
        public int Cost { get; set; }
        public int Count { get; set; }
        public int CycleDuration { get; set; }
        public int CycleDeadzoneStart { get; set; }
        public int CycleDeadzoneEnd { get; set; }
        public bool TopSection { get; set; }
        public bool SpecialOffer { get; set; }
        public int DurationSecs { get; set; }
        public int AvailabilitySecs { get; set; }
        public bool SyncToShopCycle { get; set; }
        public string Chest { get; set; }
        public int TrophyLimit { get; set; }
        public string IAP { get; set; }
        public string StarterPack_Item0_Type { get; set; }
        public string StarterPack_Item0_ID { get; set; }
        public int StarterPack_Item0_Param1 { get; set; }
        public string StarterPack_Item1_Type { get; set; }
        public string StarterPack_Item1_ID { get; set; }
        public int StarterPack_Item1_Param1 { get; set; }
        public string StarterPack_Item2_Type { get; set; }
        public string StarterPack_Item2_ID { get; set; }
        public int StarterPack_Item2_Param1 { get; set; }
        public int ValueMultiplier { get; set; }
        public bool AppendArenaToChestName { get; set; }
        public string TiedToArenaUnlock { get; set; }
        public string RepeatPurchaseGemPackOverride { get; set; }
        public string EventName { get; set; }
        public bool CostAdjustBasedOnChestContents { get; set; }
        public bool IsChronosOffer { get; set; }
    }
}