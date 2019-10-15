using System;
using System.Numerics;
using ClashRoyale.Simulator.Cards;

namespace ClashRoyale.Simulator.Types
{
    public class TroopContext
    {
        public Troop Troop { get; set; }
        public Vector2 Position { get; set; }

        public void Tick()
        {
            if (Troop == null)
            {
                Console.WriteLine("TROOP NULL!");
                return;
            }

            if (Troop.SpawnDamage > 0)
            {
                Console.WriteLine("SpawnDamage not supported yet.");
            }

            if (Troop.AreaDamage > 0)
            {
                Console.WriteLine("AreaDamage not supported yet.");
            }

            if (Troop.JumpDamage > 0)
            {
                Console.WriteLine("JumpDamage not supported yet.");
            }

            if (Position.Y + Troop.Speed - 25099 > 0 && Position.X - 1000 > 0 && Position.X < 5000)
            {
                Console.WriteLine("Hit tower!");
            }
            else
            {
                Move(0, Troop.Speed);
            }
        }

        public void Move(int x, int y)
        {
            var newX = Position.X + x;
            var newY = Position.Y + y;

            Position = new Vector2(newX, newY);

            Console.WriteLine($"{Troop.GetType().Name} moved to {Position}");
        }

        public static TroopContext Create(int id, int x, int y)
        {
            var ctx = new TroopContext
            {
                Position = new Vector2(x, y)
            };

            switch (id)
            {
                case 1:
                {
                    ctx.Troop = new Knight();
                    break;
                }
            }

            return ctx;
        }
    }
}