using System.Collections.Generic;

namespace RayForge.World
{
    // Клас EntityManager відповідає за створення, зберігання та оновлення всіх об'єктів у грі
    public sealed class EntityManager
    {
        // Список усіх сутностей (ворогів, дверей тощо), які існують у ігровому світі
        private readonly List<Entity> _entities = new();

        // Публічна властивість для отримання списку сутностей тільки для читання (захист від випадкових змін)
        public IReadOnlyList<Entity> All => _entities;

        // Конструктор: при створенні менеджера автоматично створюються стандартні об'єкти
        public EntityManager()
        {
            SpawnDefaults();
        }

        // Метод для розміщення початкових об'єктів на ігровій карті
        private void SpawnDefaults()
        {
            // Додаємо ворога за координатами X=8.5, Y=8.5
            _entities.Add(new Enemy(8.5f, 8.5f));

            // Додаємо двері за координатами X=6.5, Y=3.5
            _entities.Add(new Door(6.5f, 3.5f));
        }

        // Метод для оновлення стану всіх об'єктів кожного кадру гри
        public void Update(GameWorld world, float deltaTime)
        {
            // Перебираємо кожну сутність у списку
            foreach (Entity entity in _entities)
            {
                // Якщо об'єкт позначений як неактивний (наприклад, зібрана монетка або відкриті двері) — ігноруємо його
                if (!entity.IsActive)
                    continue;

                // Викликаємо метод Update конкретної сутності для обчислення її логіки (руху, ШІ тощо)
                entity.Update(world, deltaTime);
            }
        }
    }
}