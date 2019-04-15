using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class Abilities : Data
    {
        public Abilities(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }
        public string IconFile { get; set; }
        public string TID { get; set; }
        public string AreaEffectObject { get; set; }
        public string Buff { get; set; }
        public int BuffTime { get; set; }
        public string Effect { get; set; }
    }
}