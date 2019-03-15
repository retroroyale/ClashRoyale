using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class Locations : Data
    {
        public Locations(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }

        public bool NpcOnly { get; set; }

        public bool PvpOnly { get; set; }

        public int ShadowR { get; set; }

        public int ShadowG { get; set; }

        public int ShadowB { get; set; }

        public int ShadowA { get; set; }

        public int ShadowOffsetX { get; set; }

        public int ShadowOffsetY { get; set; }

        public string Sound { get; set; }

        public string ExtraTimeMusic { get; set; }

        public int MatchLength { get; set; }

        public string WinCondition { get; set; }

        public int OvertimeSeconds { get; set; }

        public int EndScreenDelay { get; set; }

        public string FileName { get; set; }

        public string TileDataFileName { get; set; }

        public string AmbientSound { get; set; }

        public string OverlaySC { get; set; }

        public string OverlayExportName { get; set; }

        public bool CrowdEffects { get; set; }

        public string CloudFileName { get; set; }

        public string CloudExportName { get; set; }

        public int CloudMinScale { get; set; }

        public int CloudMaxScale { get; set; }

        public int CloudMinSpeed { get; set; }

        public int CloudMaxSpeed { get; set; }

        public int CloudMinAlpha { get; set; }

        public int CloudMaxAlpha { get; set; }

        public int CloudCount { get; set; }

        public string WalkEffect { get; set; }

        public string WalkEffectOvertime { get; set; }

        public string LoopingEffectRegularTime { get; set; }

        public string LoopingEffectOvertime { get; set; }

        public string LoopingEffect { get; set; }

        public string LoopingEffectOvertimeSide { get; set; }

        public int ReflectionRed { get; set; }

        public int ReflectionGreen { get; set; }

        public int ReflectionBlue { get; set; }
    }
}