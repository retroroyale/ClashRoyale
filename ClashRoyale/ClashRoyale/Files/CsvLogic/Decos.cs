using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class Decos : Data
    {
        public Decos(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }
        public string FileName { get; set; }
        public string ExportName { get; set; }
        public string Layer { get; set; }
        public string LowendLayer { get; set; }
        public int ShadowScale { get; set; }
        public int ShadowX { get; set; }
        public int ShadowY { get; set; }
        public int ShadowSkew { get; set; }
        public int CollisionRadius { get; set; }
        public string Effect { get; set; }
        public string AssetMinTrophy { get; set; }
        public int AssetMinTrophyScore { get; set; }
        public string AssetMinTrophyFileName { get; set; }
        public int SortValue { get; set; }
    }
}