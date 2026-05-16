using System;
using RayForge.Math;

namespace RayForge.World
{
    public class Item : Entity
    {
        public bool IsCollected { get; private set; }
        public Item(float x, float y) : base(x, y)
        {
            IsActive = true;
        }
        public override void Update(GameWorld world, float deltaTime)
        {
            if (IsCollected || !IsActive) return;
            float dist = (Position - world.Player.Position).Length;
            if (dist < 0.6f)
            {
                IsCollected = true;
                IsActive = false;
                Console.WriteLine("Предмет зібрано!");
            }
        }
    }
}