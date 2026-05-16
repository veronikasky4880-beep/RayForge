using System.Collections.Generic;

namespace RayForge.World
{
    public sealed class EntityManager
    {
        private readonly List<Entity> _entities = new();
        public IReadOnlyList<Entity> All => _entities;
        public EntityManager()
        {
            SpawnDefaults();
        }
        private void SpawnDefaults()
        {
            _entities.Add(new Enemy(8.5f, 8.5f));
            _entities.Add(new Door(6.5f, 3.5f));
        }
        public void Update(GameWorld world, float deltaTime)
        {
            foreach (Entity entity in _entities)
            {
                if (!entity.IsActive)
                    continue;
                entity.Update(world, deltaTime);
            }
        }
    }
}