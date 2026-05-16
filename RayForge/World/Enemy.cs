using RayForge.Math;

namespace RayForge.World
{
    public sealed class Enemy : Entity
    {
        private const float MoveSpeed = 1.0f;
        private const float StopDistance = 0.8f;
        public Enemy(float x, float y) : base(x, y)
        {
        }
        public override void Update(GameWorld world, float deltaTime)
        {
            Vector2D direction = world.Player.Position - Position;
            float distance = direction.Length;
            if (distance < 1.8f) return;
            Vector2D moveDir = direction.Normalized();
            float nextX = Position.X + moveDir.X * MoveSpeed * deltaTime;
            float nextY = Position.Y + moveDir.Y * MoveSpeed * deltaTime;
            float r = 0.25f;
            bool canMoveX = !world.Map.IsWall((int)(nextX + r), (int)(Position.Y + r)) &&
                            !world.Map.IsWall((int)(nextX - r), (int)(Position.Y - r)) &&
                            !world.Map.IsWall((int)(nextX + r), (int)(Position.Y - r)) &&
                            !world.Map.IsWall((int)(nextX - r), (int)(Position.Y + r));
            if (canMoveX) Position = new Vector2D(nextX, Position.Y);
            bool canMoveY = !world.Map.IsWall((int)(Position.X + r), (int)(nextY + r)) &&
                            !world.Map.IsWall((int)(Position.X - r), (int)(nextY - r)) &&
                            !world.Map.IsWall((int)(Position.X + r), (int)(nextY - r)) &&
                            !world.Map.IsWall((int)(Position.X - r), (int)(nextY + r));
            if (canMoveY) Position = new Vector2D(Position.X, nextY);
        }
    }
}