using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvClient
{
    public class HealthBars : Data
    {
        public HealthBars(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row, 40);
        }

        public string Name { get; set; }
        public string FileName { get; set; }
        public string PlayerExportName { get; set; }
        public string EnemyExportName { get; set; }
        public string NoDamagePlayerExportName { get; set; }
        public string NoDamageEnemyExportName { get; set; }
        public int MinimumHitpointValue { get; set; }
        public bool ShowOwnAlways { get; set; }
        public bool ShowEnemyAlways { get; set; }
        public int YOffset { get; set; }
        public bool ShowAsShield { get; set; }
    }
}