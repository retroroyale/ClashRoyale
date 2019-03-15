using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class Taunts : Data
    {
        public Taunts(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }

        public string TID { get; set; }

        public bool TauntMenu { get; set; }

        public string FileName { get; set; }

        public string ExportName { get; set; }

        public string IconExportName { get; set; }

        public string BtnExportName { get; set; }

        public string Sound { get; set; }
    }
}