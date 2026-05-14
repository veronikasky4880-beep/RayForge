using RayForge.Math;

namespace RayForge.World
{
    // Клас Enemy представляє ворога, який може пересуватися в ігровому світі
    public sealed class Enemy : Entity
    {
        // Швидкість пересування ворога
        private const float MoveSpeed = 1.0f;

        // Дистанція, на якій ворог зупиниться перед гравцем (щоб не стояти впритул)
        private const float StopDistance = 0.8f;

        // Конструктор: встановлює початкові координати ворога
        public Enemy(float x, float y) : base(x, y)
        {
        }
        // Метод оновлення стану ворога кожного кадру (ШІ переслідування)
        public override void Update(GameWorld world, float deltaTime)
        {
            // Обчислення вектора напрямку від ворога до гравця
            Vector2D direction = world.Player.Position - Position;

            // Розрахунок відстані до гравця
            float distance = direction.Length;

            // Якщо ворог ближче ніж 1.8 метри, він зупиняється і не йде далі
            if (distance < 1.8f) return;

            // Створення нормалізованого вектора руху (напрямок з довжиною 1)
            Vector2D moveDir = direction.Normalized();

            // Розрахунок наступних потенційних координат X та Y
            float nextX = Position.X + moveDir.X * MoveSpeed * deltaTime;
            float nextY = Position.Y + moveDir.Y * MoveSpeed * deltaTime;

            // Радіус колізії ворога (його "товщина"), щоб він не заходив у стіни
            float r = 0.25f;

            // Перевірка колізії по осі X: перевіряємо всі чотири кути "квадрата" ворога
            bool canMoveX = !world.Map.IsWall((int)(nextX + r), (int)(Position.Y + r)) &&
                            !world.Map.IsWall((int)(nextX - r), (int)(Position.Y - r)) &&
                            !world.Map.IsWall((int)(nextX + r), (int)(Position.Y - r)) &&
                            !world.Map.IsWall((int)(nextX - r), (int)(Position.Y + r));

            // Якщо шлях по X вільний, оновлюємо координату X
            if (canMoveX) Position = new Vector2D(nextX, Position.Y);

            // Перевірка колізії по осі Y: аналогічна перевірка кутів для нової координати Y
            bool canMoveY = !world.Map.IsWall((int)(Position.X + r), (int)(nextY + r)) &&
                            !world.Map.IsWall((int)(Position.X - r), (int)(nextY - r)) &&
                            !world.Map.IsWall((int)(Position.X + r), (int)(nextY - r)) &&
                            !world.Map.IsWall((int)(Position.X - r), (int)(nextY + r));

            // Якщо шлях по Y вільний, оновлюємо координату Y
            if (canMoveY) Position = new Vector2D(Position.X, nextY);
        }
    }
}