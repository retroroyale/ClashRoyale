using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class GameModes : Data
    {
        public GameModes(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }

        public string TID { get; set; }

        public string RequestTID { get; set; }

        public string InProgressTID { get; set; }

        public string CardLevelAdjustment { get; set; }

        public int PlayerCount { get; set; }

        public string DeckSelection { get; set; }

        public int OvertimeSeconds { get; set; }

        public string PredefinedDecks { get; set; }

        public int ElixirProductionMultiplier { get; set; }

        public int ElixirProductionOvertimeMultiplier { get; set; }

        public bool UseStartingElixir { get; set; }

        public int StartingElixir { get; set; }

        public bool Heroes { get; set; }

        public string ForcedDeckCards { get; set; }

        public string Players { get; set; }

        public string EventDeckSetLimit { get; set; }

        public bool ForcedDeckCardsUsingCardTheme { get; set; }

        public string PrincessSkin { get; set; }

        public string KingSkin { get; set; }

        public bool GivesClanScore { get; set; }
    }
}