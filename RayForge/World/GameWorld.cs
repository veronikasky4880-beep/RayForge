using System.Collections.Generic; 
using RayForge.Core;
using RayForge.Math;
namespace RayForge.World
{
    public sealed class GameWorld
    {
        public GameMap Map { get; }
        public Player Player { get; }
        public List<Entity> Entities { get; } = new List<Entity>();
        public GameWorld()
        {
            Map = new GameMap(); 
            Player = new Player(
                x: 1.5f,
                y: 1.5f,
                angle: AngleHelper.ToRadians(0f));
        }
        public void Update(InputController input, float deltaTime)
        {
            Player.Update(input, Map, deltaTime);
            foreach (var entity in Entities)
            {
                if (entity.IsActive)
                {
                    entity.Update(this, deltaTime);
                }
            }
        }
    }
}