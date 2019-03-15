using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvClient
{
    public class Music : Data
    {
        public Music(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }

        public string FileName { get; set; }

        public int Volume { get; set; }

        public bool Loop { get; set; }

        public int PlayCount { get; set; }

        public int FadeOutTimeSec { get; set; }

        public int DurationSec { get; set; }
    }
}