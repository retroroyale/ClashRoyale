using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class AreaEffectObjects : Data
    {
        public AreaEffectObjects(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }

        public string Rarity { get; set; }

        public int LifeDuration { get; set; }

        public int LifeDurationIncreasePerLevel { get; set; }

        public int LifeDurationIncreaseAfterTournamentCap { get; set; }

        public bool AffectsHidden { get; set; }

        public int Radius { get; set; }

        public string LoopingEffect { get; set; }

        public string OneShotEffect { get; set; }

        public string ScaledEffect { get; set; }

        public string HitEffect { get; set; }

        public int Pushback { get; set; }

        public int MaximumTargets { get; set; }

        public int HitSpeed { get; set; }

        public int Damage { get; set; }

        public bool NoEffectToCrownTowers { get; set; }

        public int CrownTowerDamagePercent { get; set; }

        public bool HitBiggestTargets { get; set; }

        public string Buff { get; set; }

        public int BuffTime { get; set; }

        public int BuffTimeIncreasePerLevel { get; set; }

        public int BuffTimeIncreaseAfterTournamentCap { get; set; }

        public bool CapBuffTimeToAreaEffectTime { get; set; }

        public int BuffNumber { get; set; }

        public bool OnlyEnemies { get; set; }

        public bool OnlyOwnTroops { get; set; }

        public bool IgnoreBuildings { get; set; }

        public string Projectile { get; set; }

        public string SpawnCharacter { get; set; }

        public int SpawnInterval { get; set; }

        public string SpawnEffect { get; set; }

        public string SpawnDeployBaseAnim { get; set; }

        public int SpawnTime { get; set; }

        public int SpawnCharacterLevelIndex { get; set; }

        public int SpawnInitialDelay { get; set; }

        public int SpawnMaxCount { get; set; }

        public bool HitsGround { get; set; }

        public bool HitsAir { get; set; }

        public int ProjectileStartHeight { get; set; }

        public bool ProjectilesToCenter { get; set; }

        public string SpawnsAEO { get; set; }

        public bool ControlsBuff { get; set; }

        public bool Clone { get; set; }

        public int AttractPercentage { get; set; }
    }
}