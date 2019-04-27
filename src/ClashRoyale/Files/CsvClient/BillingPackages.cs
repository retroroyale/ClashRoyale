using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvClient
{
    public class BillingPackages : Data
    {
        public BillingPackages(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row, 2);
        }

        public string Name { get; set; }
        public string TID { get; set; }
        public bool Disabled { get; set; }
        public bool ExistsApple { get; set; }
        public bool ExistsAndroid { get; set; }
        public bool ExistsKunlun { get; set; }
        public bool ExistsJupiter { get; set; }
        public int Diamonds { get; set; }
        public int USD { get; set; }
        public int RMB { get; set; }
        public int Order { get; set; }
        public string IconFile { get; set; }
        public string JupiterID { get; set; }
        public string StarterPackName { get; set; }
        public bool IsRedPackage { get; set; }
        public string RumblePackName { get; set; }
        public string ChronosOfferName { get; set; }
        public int RedeemMax { get; set; }
        public int CampaignId { get; set; }
    }
}