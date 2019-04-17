using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvClient
{
    public class News : Data
    {
        public News(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row, 58);
        }

        public string Name { get; set; }
        public int ID { get; set; }
        public bool Enabled { get; set; }
        public string TID { get; set; }
        public string InfoTID { get; set; }
        public string ItemSWF { get; set; }
        public string ItemExportName { get; set; }
        public string IconSWF { get; set; }
        public string IconExportName { get; set; }
        public string ImageSWF { get; set; }
        public string ImageExportName { get; set; }
        public string ButtonUrl { get; set; }
        public string ButtonTID { get; set; }
    }
}