using System.Collections.Generic; // Важливо! Для роботи зі динамічними списками
using RayForge.Core;
using RayForge.Math;

namespace RayForge.World
{
    // Клас GameWorld є контейнером, який об'єднує карту, гравця та всіх ворогів в одну систему
    public sealed class GameWorld
    {
        // Посилання на об'єкт карти рівня
        public GameMap Map { get; }

        // Посилання на об'єкт головного героя (гравця)
        public Player Player { get; }

        // Список усіх сутностей (ворогів, предметів, дверей), які існують у цьому світі
        public List<Entity> Entities { get; } = new List<Entity>();

        // Конструктор: ініціалізує карту та встановлює гравця в початкову позицію
        public GameWorld()
        {
            Map = new GameMap(); // Створення нової карти

            // Створення гравця з початковими координатами (1.5, 1.5) та поглядом на схід (0 градусів)
            Player = new Player(
                x: 1.5f,
                y: 1.5f,
                angle: AngleHelper.ToRadians(0f));
        }

        // Головний метод оновлення логіки всього ігрового світу
        public void Update(InputController input, float deltaTime)
        {
            // Оновлення стану гравця (обробка натискань клавіш, рух, перевірка зіткнень з картою)
            Player.Update(input, Map, deltaTime);

            // Оновлення логіки кожної сутності (ворога або предмета) у світі
            foreach (var entity in Entities)
            {
                // Оновлюємо сутність лише в тому випадку, якщо вона активна
                if (entity.IsActive)
                {
                    // Передаємо посилання на весь ігровий світ (this) та час кадру для розрахунків ШІ
                    entity.Update(this, deltaTime);
                }
            }
        }
    }
}