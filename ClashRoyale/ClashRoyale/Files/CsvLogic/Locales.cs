using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class Locales : Data
    {
        public Locales(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }

        public bool Enabled { get; set; }

        public string Description { get; set; }

        public int SortOrder { get; set; }

        public bool HasEvenSpaceCharacters { get; set; }

        public string UsedSystemFont { get; set; }

        public string HelpshiftSDKLanguage { get; set; }

        public string HelpshiftSDKLanguageAndroid { get; set; }

        public string HelpshiftLanguageTag { get; set; }

        public string TermsAndServiceUrl { get; set; }

        public string ParentsGuideUrl { get; set; }

        public string PrivacyPolicyUrl { get; set; }

        public bool TestLanguage { get; set; }

        public string TestExcludes { get; set; }

        public string RegionListFile { get; set; }

        public bool MaintenanceRoyalBox { get; set; }

        public string RoyalBoxURL { get; set; }

        public string RoyalBoxStageURL { get; set; }

        public string RoyalBoxDevURL { get; set; }

        public string BoomBoxURL { get; set; }

        public string EventsURL { get; set; }
    }
}