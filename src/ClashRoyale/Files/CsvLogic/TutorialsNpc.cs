using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class TutorialsNpc : Data
    {
        public TutorialsNpc(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row, 48);
        }

        public string Name { get; set; }
        public string Location { get; set; }
        public string NPC { get; set; }
        public string TID { get; set; }
        public string ButtonTID { get; set; }
        public string FinishRequirement { get; set; }
        public string Chest { get; set; }
        public int WaitTimeMS { get; set; }
        public string FileName { get; set; }
        public int PopupCorner { get; set; }
        public string PopupExportName { get; set; }
        public string BubbleExportName { get; set; }
        public string Sound { get; set; }
        public bool Darkening { get; set; }
        public string BubbleObject { get; set; }
        public string OverlayExportName { get; set; }
        public string SpellDragExportName { get; set; }
        public string SpellToCast { get; set; }
        public bool ForceSpellTile { get; set; }
        public bool DisableOtherSpells { get; set; }
        public int SpellTileX { get; set; }
        public int SpellTileY { get; set; }
        public bool DisableSpells { get; set; }
        public bool HideCombatUI { get; set; }
        public bool DisableTroopMovement { get; set; }
        public bool DisableLeaderMovement { get; set; }
        public bool DisableSpawnPoints { get; set; }
        public bool DisableOpponentSpells { get; set; }
        public bool PauseCombat { get; set; }
        public string Dependency { get; set; }
        public int Priority { get; set; }
        public string Taunt { get; set; }
        public bool HighlightTargetsOnManaFull { get; set; }
        public bool DisableBattleStartScreen { get; set; }
        public int NpcMatchesPlayed { get; set; }
        public bool DisableBattleMenu { get; set; }
        public int CloseAutomaticallyAfterSeconds { get; set; }
        public int GroupMod { get; set; }
        public int GroupValue { get; set; }
    }
}