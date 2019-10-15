namespace ClashRoyale.Simulator.Types
{
    public class Troop
    {
        public int Level { get; set; }

        public int Hitpoints { get; set; } 
        public double HitSpeed { get; set; } 

        public int JumpDamage { get; set; }
        public int AreaDamage { get; set; }
        public int SpawnDamage { get; set; }
        public int Damage { get; set; } 
        public int DamagePerSecond { get; set; } 

        public int SightRange { get; set; } 
        public double Range { get; set; } 
        public int Count { get; set; } 

        public int LoadTime { get; set; } 
        public int DeployTime { get; set; } 
        public int Speed { get; set; }

        public Target Targets { get; set; }

        public enum Target
        {
            Ground = 0,
            Air = 1,
            Both = 2
        }
    }
}
