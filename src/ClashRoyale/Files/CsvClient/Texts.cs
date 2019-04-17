using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvClient
{
    public class Texts : Data
    {
        public Texts(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row, 0);
        }

        public string Column { get; set; }
        public string EN { get; set; }
        public string FR { get; set; }
        public string DE { get; set; }
        public string ES { get; set; }
        public string IT { get; set; }
        public string NL { get; set; }
        public string NO { get; set; }
        public string TR { get; set; }
        public string JP { get; set; }
        public string KR { get; set; }
        public string RU { get; set; }
        public string AR { get; set; }
        public string PT { get; set; }
        public string CN { get; set; }
        public string CNT { get; set; }
        public string FA { get; set; }
        public string ID { get; set; }
        public string MS { get; set; }
    }
}