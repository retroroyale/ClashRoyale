using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class PveWaves : Data
    {
        public PveWaves(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }

        public string Spells { get; set; }

        public int PositionX { get; set; }

        public int PositionY { get; set; }

        public int Delay { get; set; }
    }
}