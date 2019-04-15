using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class PveBoss : Data
    {
        public PveBoss(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }
        public string Waves { get; set; }
        public int WaveDuration { get; set; }
        public bool Repeat { get; set; }
    }
}