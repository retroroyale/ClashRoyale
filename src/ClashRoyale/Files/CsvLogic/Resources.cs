using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class Resources : Data
    {
        public Resources(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row, 5);
        }

        public string Name { get; set; }
        public string TID { get; set; }
        public string IconSWF { get; set; }
        public bool UsedInBattle { get; set; }
        public string CollectEffect { get; set; }
        public string IconExportName { get; set; }
        public bool PremiumCurrency { get; set; }
        public string CapFullTID { get; set; }
        public int TextRed { get; set; }
        public int TextGreen { get; set; }
        public int TextBlue { get; set; }
        public int Cap { get; set; }
        public string IconFile { get; set; }
        public string ShopIcon { get; set; }
    }
}