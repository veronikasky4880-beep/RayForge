using System.Windows.Forms;
using RayForge.Core;
using RayForge.Math;

namespace RayForge.World
{
    // Клас Player описує стан гравця, його пересування та взаємодію зі світом
    public sealed class Player
    {
        // Константи швидкості: руху вперед/назад та повороту камери
        private const float MoveSpeed = 4.5f;
        private const float RotationSpeed = 2.8f;

        // Позиція гравця у 2D просторі (X, Y)
        public Vector2D Position { get; private set; }

        // Кут повороту гравця в радіанах
        public float Angle { get; private set; }

        // Обчислювана властивість: вектор напрямку погляду, отриманий із кута
        public Vector2D Direction =>
            Vector2D.FromAngle(Angle);

        // Конструктор: задає початкову позицію та кут погляду
        public Player(float x, float y, float angle)
        {
            Position = new Vector2D(x, y);
            Angle = angle;
        }

        // Головний метод оновлення стану гравця кожного кадру
        public void Update(InputController input, GameMap map, float deltaTime)
        {
            UpdateRotation(input, deltaTime); // Оновлюємо кут повороту
            UpdateMovement(input, map, deltaTime); // Обчислюємо рух
        }

        // Обробка обертання вліво та вправо
        private void UpdateRotation(InputController input, float deltaTime)
        {
            // Клавіші Left/A повертають камеру вліво
            if (input.IsKeyDown(Keys.Left) || input.IsKeyDown(Keys.A))
                Angle -= RotationSpeed * deltaTime;

            // Клавіші Right/D повертають камеру вправо
            if (input.IsKeyDown(Keys.Right) || input.IsKeyDown(Keys.D))
                Angle += RotationSpeed * deltaTime;

            // Нормалізація кута (щоб він завжди був у межах 0..2*Pi)
            Angle = AngleHelper.Normalize(Angle);
        }

        // Обробка натискання клавіш руху
        private void UpdateMovement(InputController input, GameMap map, float deltaTime)
        {
            Vector2D movement = new();

            // Рух вперед (W/Up) додає вектор напрямку
            if (input.IsKeyDown(Keys.W) || input.IsKeyDown(Keys.Up))
                movement += Direction;

            // Рух назад (S/Down) віднімає вектор напрямку
            if (input.IsKeyDown(Keys.S) || input.IsKeyDown(Keys.Down))
                movement -= Direction;

            // Якщо ніякі клавіші не натиснуті — виходимо
            if (movement.Length <= 0.001f)
                return;

            // Нормалізуємо вектор руху (щоб рух по діагоналі не був швидшим) 
            // та множимо на швидкість і час кадру
            movement = movement.Normalized() * MoveSpeed * deltaTime;

            // Спроба перемістити гравця з урахуванням колізій
            TryMove(map, movement);
        }

        // Метод перевірки зіткнень зі стінами (колізія)
        private void TryMove(GameMap map, Vector2D offset)
        {
            // Радіус гравця ("товщина"). Дозволяє не входити в стіни впритул.
            float r = 0.2f;

            // Перевірка руху по осі X
            float nextX = Position.X + offset.X;
            // Перевіряємо дві точки (верхній та нижній краї "радіуса" гравця)
            bool canMoveX = !map.IsWall(new Vector2D(nextX + (offset.X > 0 ? r : -r), Position.Y + r)) &&
                            !map.IsWall(new Vector2D(nextX + (offset.X > 0 ? r : -r), Position.Y - r));

            if (canMoveX)
                Position = new Vector2D(nextX, Position.Y);

            // Перевірка руху по осі Y
            float nextY = Position.Y + offset.Y;
            // Перевіряємо дві точки (лівий та правий краї "радіуса" гравця)
            bool canMoveY = !map.IsWall(new Vector2D(Position.X + r, nextY + (offset.Y > 0 ? r : -r))) &&
                            !map.IsWall(new Vector2D(Position.X - r, nextY + (offset.Y > 0 ? r : -r)));

            if (canMoveY)
                Position = new Vector2D(Position.X, nextY);
        }
    }
}