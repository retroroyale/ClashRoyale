using ClashRoyale.Simulator.Types;

namespace ClashRoyale.Simulator.Cards
{
    public class Knight : Troop
    {
        public Knight()
        {
            Level = 1;
            Hitpoints = 660;
            HitSpeed = 1100;
            Damage = 75;
            DamagePerSecond = 68;
            SightRange = 5500;
            Range = 1000;
            Count = 1;
            LoadTime = 700;
            DeployTime = 1000;
            Speed = 60;
            Targets = Target.Ground;
        }
    }
}
