using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class Projectiles : Data
    {
        public Projectiles(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }

        public string Rarity { get; set; }

        public int Speed { get; set; }

        public string FileName { get; set; }

        public string ExportName { get; set; }

        public string RedExportName { get; set; }

        public string ShadowExportName { get; set; }

        public string RedShadowExportName { get; set; }

        public bool ShadowDisableRotate { get; set; }

        public int Scale { get; set; }

        public bool Homing { get; set; }

        public string HitEffect { get; set; }

        public string DeathEffect { get; set; }

        public int Damage { get; set; }

        public int CrownTowerDamagePercent { get; set; }

        public int Pushback { get; set; }

        public bool PushbackAll { get; set; }

        public int Radius { get; set; }

        public int RadiusY { get; set; }

        public bool AoeToAir { get; set; }

        public bool AoeToGround { get; set; }

        public bool OnlyEnemies { get; set; }

        public bool OnlyOwnTroops { get; set; }

        public int MaximumTargets { get; set; }

        public int Gravity { get; set; }

        public string SpawnAreaEffectObject { get; set; }

        public int SpawnCharacterLevelIndex { get; set; }

        public int SpawnCharacterDeployTime { get; set; }

        public string SpawnCharacter { get; set; }

        public bool SpawnConstPriority { get; set; }

        public int SpawnCharacterCount { get; set; }

        public string TargetBuff { get; set; }

        public int BuffTime { get; set; }

        public int BuffTimeIncreasePerLevel { get; set; }

        public string TrailEffect { get; set; }

        public int ProjectileRadius { get; set; }

        public int ProjectileRadiusY { get; set; }

        public int ProjectileRange { get; set; }

        public bool use360Frames { get; set; }

        public string HitSoundWhenParentAlive { get; set; }

        public string SpawnProjectile { get; set; }

        public int MinDistance { get; set; }

        public int MaxDistance { get; set; }

        public int ConstantHeight { get; set; }

        public bool HeightFromTargetRadius { get; set; }

        public int Heal { get; set; }

        public int CrownTowerHealPercent { get; set; }

        public bool TargetToEdge { get; set; }

        public int ChainedHitRadius { get; set; }

        public string ChainedHitEndEffect { get; set; }

        public string PingpongDeathEffect { get; set; }

        public bool ShakesTargets { get; set; }

        public int PingpongVisualTime { get; set; }

        public int RandomAngle { get; set; }

        public int RandomDistance { get; set; }

        public string Scatter { get; set; }

        public int DragBackSpeed { get; set; }

        public int DragMargin { get; set; }

        public string TargettedEffect { get; set; }
    }
}