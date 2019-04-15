using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvClient
{
    public class ParticleEmitters : Data
    {
        public ParticleEmitters(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }

        public int ParticleCount { get; set; }

        public int MinLife { get; set; }

        public int MaxLife { get; set; }

        public int ParticleMinInterval { get; set; }

        public int ParticleMaxInterval { get; set; }

        public int ParticleMinLife { get; set; }

        public int ParticleMaxLife { get; set; }

        public int ParticleMinAngle { get; set; }

        public int ParticleMaxAngle { get; set; }

        public bool ParticleAngleRelativeToParent { get; set; }

        public bool ParticleRandomAngle { get; set; }

        public int ParticleMinRadius { get; set; }

        public int ParticleMaxRadius { get; set; }

        public int ParticleMinSpeed { get; set; }

        public int ParticleMaxSpeed { get; set; }

        public int ParticleStartXYAreaRadius { get; set; }

        public int ParticleStartZ { get; set; }

        public int ParticleMinVelocityZ { get; set; }

        public int ParticleMaxVelocityZ { get; set; }

        public int ParticleGravity { get; set; }

        public int ParticleMinTailLength { get; set; }

        public int ParticleMaxTailLength { get; set; }

        public string ParticleResource { get; set; }

        public string ParticleExportName { get; set; }

        public bool RotateToDirection { get; set; }

        public bool LoopParticleClip { get; set; }

        public int StartScale { get; set; }

        public int EndScale { get; set; }

        public int FadeInDuration { get; set; }

        public int FadeOutDuration { get; set; }

        public int Inertia { get; set; }

        public string EnemyVersion { get; set; }

        public bool NoBounce { get; set; }

        public bool StopOnBounce { get; set; }

        public int RandomScale { get; set; }

        public bool NoLowEndOptimization { get; set; }

        public int SortingOffset { get; set; }

        public bool Shadow { get; set; }

        public int AngularSpeed { get; set; }

        public int ShadowMulR { get; set; }

        public int ShadowMulG { get; set; }

        public int ShadowMulB { get; set; }

        public int ShadowMulA { get; set; }

        public bool InverseSpeed { get; set; }

        public bool Trail { get; set; }

        public int TrailWidth { get; set; }

        public int TrailMaxPoints { get; set; }

        public int TrailDuration { get; set; }

        public string TrailSWF { get; set; }

        public string TrailExportName { get; set; }

        public string SpecialEffect { get; set; }

        public bool FrameFromAngle { get; set; }

        public int RotateMinSpeed { get; set; }

        public int RotateMaxSpeed { get; set; }

        public bool IgnoreShadowFlip { get; set; }

        public bool ResourceFromAngle { get; set; }
    }
}