using RayForge.Math;

namespace RayForge.World
{
    public sealed class GameMap
    {
        private readonly int[,] _tiles =
        {
            { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,1,1,0,1,1,1,0,1,1,1,2,1,0,1 }, // 
            { 1,0,1,0,0,0,0,1,0,0,0,1,0,1,0,1 },
            { 1,0,1,0,1,1,2,1,1,1,0,1,0,1,0,1 }, // 
            { 1,0,0,0,1,0,0,0,0,1,0,0,0,1,0,1 },
            { 1,1,1,2,1,0,1,1,0,1,1,1,0,1,0,1 }, // 
            { 1,0,0,0,0,0,1,0,0,0,0,1,0,0,0,1 },
            { 1,0,1,1,1,0,1,0,1,1,0,1,1,1,0,1 },
            { 1,0,0,0,1,0,0,0,1,0,0,0,0,1,0,1 },
            { 1,0,1,0,1,1,1,2,1,0,1,1,0,1,0,1 }, //
            { 1,0,1,0,0,0,1,0,0,0,1,0,0,0,0,1 },
            { 1,0,1,1,1,0,1,1,1,0,1,0,1,1,0,1 },
            { 1,0,0,0,0,0,0,0,1,0,0,0,0,1,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }
        };

        public int Width => _tiles.GetLength(1);
        public int Height => _tiles.GetLength(0);
        public bool IsWall(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return true;
            return _tiles[y, x] != 0;
        }
        public bool IsWall(Vector2D position)
        {
            return IsWall(
                (int)position.X,
                (int)position.Y);
        }
        public int GetTile(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return 1;
            return _tiles[y, x];
        }
    }
}