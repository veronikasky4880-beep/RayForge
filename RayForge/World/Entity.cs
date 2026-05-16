using RayForge.Math;

namespace RayForge.World
{
    public abstract class Entity
    {
        public Vector2D Position { get; protected set; }
        public bool IsActive { get; set; } = true;
        public int TextureId { get; set; } = 10;
        protected Entity(float x, float y)
        {
            Position = new Vector2D(x, y);
        }
        public virtual void Update(GameWorld world, float deltaTime) { }
    }
}