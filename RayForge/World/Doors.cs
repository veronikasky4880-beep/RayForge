using RayForge.Math;
namespace RayForge.World
{
    public sealed class Door : Entity
    {
        private const float OpenSpeed = 1.5f;
        private const float MaxOpenAmount = 1f;
        public bool IsOpening { get; private set; }
        public float OpenAmount { get; private set; }
        public Door(float x, float y) : base(x, y)
        {
        }
        public void Open()
        {
            IsOpening = true;
        }
        public override void Update(GameWorld world, float deltaTime)
        {
            if (!IsOpening)
                return;
            OpenAmount += OpenSpeed * deltaTime;
            if (OpenAmount >= MaxOpenAmount)
            {
                OpenAmount = MaxOpenAmount; 
                IsOpening = false;          
                IsActive = false;
            }
        }
    }
}