using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvClient
{
    public class Hints : Data
    {
        public Hints(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }

        public string TID { get; set; }

        public bool NotBeenInClan { get; set; }

        public bool NotBeenInTournament { get; set; }

        public bool NotCreatedTournament { get; set; }

        public int MinNpcWins { get; set; }

        public int MaxNpcWins { get; set; }

        public int MinArena { get; set; }

        public int MaxArena { get; set; }

        public int MinTrophies { get; set; }

        public int MaxTrophies { get; set; }

        public int MinExpLevel { get; set; }

        public int MaxExpLevel { get; set; }

        public string iOSTID { get; set; }

        public string AndroidTID { get; set; }
    }
}