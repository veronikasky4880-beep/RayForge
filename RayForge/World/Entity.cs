using RayForge.Math;

namespace RayForge.World
{
    // Абстрактний клас Entity — це фундамент для всіх об'єктів у грі (гравця, ворогів, предметів)
    public abstract class Entity
    {
        // Поточна позиція об'єкта у світі (X, Y). Доступ до зміни є лише у спадкоємців.
        public Vector2D Position { get; protected set; }

        // Прапорець активності об'єкта. Якщо false — об'єкт не оновлюється і не малюється.
        public bool IsActive { get; set; } = true;

        // Ідентифікатор текстури, яка буде використовуватися для візуалізації цього об'єкта
        public int TextureId { get; set; } = 10;

        // Конструктор: створює сутність у заданих координатах
        protected Entity(float x, float y)
        {
            Position = new Vector2D(x, y);
        }

        // Віртуальний метод оновлення логіки, який кожен тип об'єкта може реалізувати по-своєму
        public virtual void Update(GameWorld world, float deltaTime) { }
    }
}