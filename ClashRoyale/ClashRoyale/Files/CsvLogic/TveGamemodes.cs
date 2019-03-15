using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class TveGamemodes : Data
    {
        public TveGamemodes(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }

        public string PrimarySpells { get; set; }

        public string SecondarySpells { get; set; }

        public string CastSpells { get; set; }

        public bool RandomWaves { get; set; }

        public int ElixirPerWave { get; set; }

        public int WaveCount { get; set; }

        public int TimePerWave { get; set; }

        public int TimeToFirstWave { get; set; }

        public string ForcedCards1 { get; set; }

        public string ForcedCards2 { get; set; }

        public bool RotateDecks { get; set; }
    }
}