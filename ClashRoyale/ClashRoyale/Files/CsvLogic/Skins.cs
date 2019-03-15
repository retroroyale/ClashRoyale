using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class Skins : Data
    {
        public Skins(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }

        public string FileName { get; set; }

        public string ExportName { get; set; }

        public string ExportNameRed { get; set; }

        public string TopExportName { get; set; }

        public string TopExportNameRed { get; set; }

        public string Category { get; set; }

        public int ValueGems { get; set; }

        public string TID { get; set; }

        public string IconSWF { get; set; }

        public string IconExportName { get; set; }

        public bool IsInUse { get; set; }

        public string Type { get; set; }
    }
}