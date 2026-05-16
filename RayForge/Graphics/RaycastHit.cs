using RayForge.Math;
namespace RayForge.Graphics
{
    public readonly struct RaycastHit
    {
        public Vector2D Position { get; }
        public float Distance { get; }
        public bool HitVerticalWall { get; }
        public int TileId { get; }
        public RaycastHit(
            Vector2D position,
            float distance,
            bool hitVerticalWall,
            int tileId)
        {
            Position = position;       
            Distance = distance;      
            HitVerticalWall = hitVerticalWall; 
            TileId = tileId;         
        }
    }
}