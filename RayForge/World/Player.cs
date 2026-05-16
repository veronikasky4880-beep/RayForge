using System.Windows.Forms;
using RayForge.Core;
using RayForge.Math;

namespace RayForge.World
{
    public sealed class Player
    {
        private const float MoveSpeed = 4.5f;
        private const float RotationSpeed = 2.8f;
        public Vector2D Position { get; private set; }
        public float Angle { get; private set; }
        public Vector2D Direction =>
            Vector2D.FromAngle(Angle);
        public Player(float x, float y, float angle)
        {
            Position = new Vector2D(x, y);
            Angle = angle;
        }
        public void Update(InputController input, GameMap map, float deltaTime)
        {
            UpdateRotation(input, deltaTime);
            UpdateMovement(input, map, deltaTime); 
        }
        private void UpdateRotation(InputController input, float deltaTime)
        {
            if (input.IsKeyDown(Keys.Left) || input.IsKeyDown(Keys.A))
                Angle -= RotationSpeed * deltaTime;
            if (input.IsKeyDown(Keys.Right) || input.IsKeyDown(Keys.D))
                Angle += RotationSpeed * deltaTime;
            Angle = AngleHelper.Normalize(Angle);
        }
        private void UpdateMovement(InputController input, GameMap map, float deltaTime)
        {
            Vector2D movement = new();
            if (input.IsKeyDown(Keys.W) || input.IsKeyDown(Keys.Up))
                movement += Direction;
            if (input.IsKeyDown(Keys.S) || input.IsKeyDown(Keys.Down))
                movement -= Direction;
            if (movement.Length <= 0.001f)
                return;
            movement = movement.Normalized() * MoveSpeed * deltaTime;
            TryMove(map, movement);
        }
        private void TryMove(GameMap map, Vector2D offset)
        {
            float r = 0.2f;
            float nextX = Position.X + offset.X;
            bool canMoveX = !map.IsWall(new Vector2D(nextX + (offset.X > 0 ? r : -r), Position.Y + r)) &&
                            !map.IsWall(new Vector2D(nextX + (offset.X > 0 ? r : -r), Position.Y - r));
            if (canMoveX)
                Position = new Vector2D(nextX, Position.Y);
            float nextY = Position.Y + offset.Y;
            bool canMoveY = !map.IsWall(new Vector2D(Position.X + r, nextY + (offset.Y > 0 ? r : -r))) &&
                            !map.IsWall(new Vector2D(Position.X - r, nextY + (offset.Y > 0 ? r : -r)));
            if (canMoveY)
                Position = new Vector2D(Position.X, nextY);
        }
    }
}