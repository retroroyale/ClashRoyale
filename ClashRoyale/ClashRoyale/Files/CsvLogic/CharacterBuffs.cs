using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class CharacterBuffs : Data
    {
        public CharacterBuffs(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }

        public string Rarity { get; set; }

        public string TID { get; set; }

        public string IconFileName { get; set; }

        public string IconExportName { get; set; }

        public bool ChangeControl { get; set; }

        public bool NoEffectToCrownTowers { get; set; }

        public int CrownTowerDamagePercent { get; set; }

        public int DamagePerSecond { get; set; }

        public int HitFrequency { get; set; }

        public int DamageReduction { get; set; }

        public int HealPerSecond { get; set; }

        public bool ImmuneToAntiMagic { get; set; }

        public int HitSpeedMultiplier { get; set; }

        public int SpeedMultiplier { get; set; }

        public int SpawnSpeedMultiplier { get; set; }

        public string NegatesBuffs { get; set; }

        public string ImmunityToBuffs { get; set; }

        public bool Invisible { get; set; }

        public bool RemoveOnAttack { get; set; }

        public bool RemoveOnHeal { get; set; }

        public int DamageMultiplier { get; set; }

        public bool Panic { get; set; }

        public string Effect { get; set; }

        public string FilterFile { get; set; }

        public string FilterExportName { get; set; }

        public bool FilterAffectsTransformation { get; set; }

        public bool FilterInheritLifeDuration { get; set; }

        public int SizeMultiplier { get; set; }

        public bool StaticTarget { get; set; }

        public bool IgnorePushBack { get; set; }

        public string MarkEffect { get; set; }

        public int AudioPitchModifier { get; set; }

        public string PortalSpell { get; set; }

        public int AttractPercentage { get; set; }

        public bool ControlledByParent { get; set; }

        public bool Clone { get; set; }

        public int Scale { get; set; }

        public bool EnableStacking { get; set; }
    }
}