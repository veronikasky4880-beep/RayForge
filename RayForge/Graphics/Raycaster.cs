using System;
using System.Collections.Generic;
using System.Drawing;
using RayForge.Math;
using RayForge.World;

namespace RayForge.Graphics
{
    // Клас Raycaster відповідає за рендеринг 3D-сцени методом трасування променів
    public sealed class Raycaster
    {
        // Менеджер текстур для завантаження та отримання зображень стін та об'єктів
        public TextureManager Textures { get; } = new();

        // Z-буфер для зберігання відстані до стін (використовується для коректного малювання спрайтів поверх стін)
        public float[] ZBuffer { get; private set; } = Array.Empty<float>();

        // Константа кута огляду (FOV) - приблизно 60 градусів
        private const float FieldOfView = MathF.PI / 3f;

        // Максимальна дальність промальовування променів
        private const float MaxDepth = 32f;

        // Основна функція малювання кадру
        public void Render(System.Drawing.Graphics graphics, GameWorld world, Size viewport)
        {
            // Перевірка та оновлення розміру Z-буфера відповідно до ширини вікна
            if (ZBuffer.Length != viewport.Width)
            {
                ZBuffer = new float[viewport.Width];
            }

            // Очищення екрана чорним кольором перед малюванням нового кадру
            graphics.Clear(Color.Black);

            // Малювання верхньої частини екрана (небо)
            DrawSky(graphics, viewport);

            // Малювання нижньої частини екрана (підлога)
            DrawFloor(graphics, viewport);

            // 1. малюємо стіни та заповнюємо ZBuffer (основний етап рейкастингу)
            RenderWalls(graphics, world, viewport);

            // 2. малюємо ворогів (спрайти), враховуючи дані з ZBuffer
            RenderSprites(graphics, world, viewport);

            // Відображення міні-карти в кутку екрана
            DrawMinimap(graphics, world);

            // Створення пера для малювання прицілу
            using var crosshair = new Pen(Color.White, 2f);

            // Обчислення координат центру екрана
            int cx = viewport.Width / 2;
            int cy = viewport.Height / 2;

            // Малювання горизонтальної лінії прицілу
            graphics.DrawLine(crosshair, cx - 8, cy, cx + 8, cy);

            // Малювання вертикальної лінії прицілу
            graphics.DrawLine(crosshair, cx, cy - 8, cx, cy + 8);
        }

        // Функція для рендерингу стін лабіринту
        private void RenderWalls(System.Drawing.Graphics graphics, GameWorld world, Size viewport)
        {
            Player player = world.Player; // Посилання на об'єкт гравця

            // Прохід по кожному другому пікселю ширини для оптимізації швидкості
            for (int column = 0; column < viewport.Width; column += 2)
            {
                // Обчислення положення променя відносно камери
                float cameraX = (float)column / viewport.Width;

                // Розрахунок кута конкретного променя в межах кута огляду
                float rayAngle = player.Angle - FieldOfView / 2f + cameraX * FieldOfView;

                // Запуск променя для пошуку зіткнення зі стіною
                RaycastHit hit = CastRay(world.Map, player.Position, rayAngle);

                // Корекція ефекту "риб'ячого ока" (множення на косинус)
                float correctedDistance = hit.Distance * MathF.Cos(rayAngle - player.Angle);

                // Захист від ділення на нуль, якщо відстань надто мала
                correctedDistance = MathF.Max(correctedDistance, 0.0001f);

                // Запис відстані до Z-буфера для поточного та сусіднього стовпців
                ZBuffer[column] = correctedDistance;
                if (column + 1 < ZBuffer.Length) ZBuffer[column + 1] = correctedDistance;

                // Обчислення висоти стіни на екрані (чим далі стіна, тим вона менша)
                int wallHeight = (int)(viewport.Height / correctedDistance);

                // Обчислення верхньої координати для центрування стіни по вертикалі
                int top = (viewport.Height - wallHeight) / 2;

                // Отримання потрібної текстури за ID плитки карти
                Bitmap texture = Textures.GetTexture(hit.TileId);

                // Визначення координати X на текстурі (залежить від місця удару променя)
                float wallX = hit.HitVerticalWall ? hit.Position.Y : hit.Position.X;
                wallX -= MathF.Floor(wallX); // Отримання лише дробової частини

                // Перетворення відносної координати стіни в піксельну координату текстури
                int texX = (int)(wallX * texture.Width);

                // Малювання вертикальної смужки текстури стіни на екрані
                graphics.DrawImage(texture,
                    new Rectangle(column, top, 2, wallHeight),
                    new Rectangle(texX, 0, 1, texture.Height),
                    GraphicsUnit.Pixel);
            }
        }

        // Функція для рендерингу ігрових об'єктів (ворогів, предметів)
        private void RenderSprites(System.Drawing.Graphics graphics, GameWorld world, Size viewport)
        {
            // Перебір усіх сутностей у ігровому світі
            foreach (var entity in world.Entities)
            {
                // Пропускаємо неактивні об'єкти
                if (!entity.IsActive) continue;

                // Розрахунок вектора від гравця до об'єкта
                Vector2D delta = entity.Position - world.Player.Position;
                float distance = delta.Length; // Відстань до об'єкта

                // Не малюємо об'єкти, які знаходяться надто близько
                if (distance < 0.2f) continue;

                // Обчислення кута напрямку на об'єкт
                float angleToEntity = MathF.Atan2(delta.Y, delta.X);

                // Різниця кутів між поглядом гравця та напрямком на об'єкт
                float relativeAngle = AngleHelper.Delta(world.Player.Angle, angleToEntity);

                // Відсікання об'єктів, що знаходяться позаду гравця (більше 90 градусів)
                if (MathF.Abs(relativeAngle) > MathF.PI / 2f) continue;

                // Корекція відстані для спрайта
                float correctedDistance = distance * MathF.Cos(relativeAngle);

                // Обчислення розмірів спрайта на екрані з коефіцієнтом росту
                float spriteHeight = (viewport.Height / correctedDistance) * 0.7f;
                float spriteWidth = spriteHeight;

                // Позиція центру спрайта на екрані по горизонталі
                float screenX = (relativeAngle / FieldOfView + 0.5f) * viewport.Width;

                // Верхня межа спрайта по вертикалі
                float top = (viewport.Height - spriteHeight) * 0.5f;

                // Отримання текстури для конкретної сутності
                Bitmap texture = Textures.GetTexture(entity.TextureId);

                // Розрахунок початкової та кінцевої X-координат спрайта на екрані
                int startX = (int)(screenX - spriteWidth / 2);
                int endX = (int)(screenX + spriteWidth / 2);

                // Попіксельне малювання спрайта по горизонталі
                for (int x = startX; x < endX; x++)
                {
                    // Перевірка, чи входить піксель у межі екрана
                    if (x < 0 || x >= viewport.Width) continue;

                    // Перевірка Z-буфера: якщо стіна ближче, ніж об'єкт, не малюємо цей піксель
                    if (ZBuffer[x] < correctedDistance - 0.1f) continue;

                    // Розрахунок поточної X-координати на текстурі
                    int texX = (int)((float)(x - startX) / spriteWidth * texture.Width);
                    if (texX < 0 || texX >= texture.Width) continue;

                    // Обмеження висоти малювання для запобігання помилкам GDI+ при великому наближенні
                    int finalH = (int)spriteHeight;
                    if (finalH > viewport.Height * 2) finalH = viewport.Height * 2;

                    if (finalH > 0)
                    {
                        // Малювання вертикальної смужки спрайта
                        graphics.DrawImage(texture,
                            new Rectangle(x, (int)top, 1, finalH),
                            new Rectangle(texX, 0, 1, texture.Height),
                            GraphicsUnit.Pixel);
                    }
                }
            }
        }

        // Малювання прямокутника неба у верхній частині вікна
        private void DrawSky(System.Drawing.Graphics graphics, Size viewport)
        {
            graphics.FillRectangle(Brushes.SteelBlue, 0, 0, viewport.Width, viewport.Height / 2);
        }

        // Малювання прямокутника підлоги у нижній частині вікна
        private void DrawFloor(System.Drawing.Graphics graphics, Size viewport)
        {
            graphics.FillRectangle(Brushes.DimGray, 0, viewport.Height / 2, viewport.Width, viewport.Height / 2);
        }

        // Метод для випуску променя через ігрову карту
        private RaycastHit CastRay(GameMap map, Vector2D origin, float angle)
        {
            Vector2D direction = Vector2D.FromAngle(angle); // Напрямок променя
            Vector2D position = origin; // Поточна позиція променя
            float distance = 0f; // Пройдена відстань
            const float step = 0.05f; // Крок ітерації променя

            // Цикл руху променя до межі або до зіткнення
            while (distance < MaxDepth)
            {
                position += direction * step; // Переміщення променя вперед
                distance += step; // Збільшення лічильника відстані

                int tileX = (int)position.X; // Координата клітинки по X
                int tileY = (int)position.Y; // Координата клітинки по Y

                // Якщо промінь потрапив у стіну
                if (map.IsWall(tileX, tileY))
                {
                    float fx = position.X - tileX; // Позиція всередині клітинки по X
                    float fy = position.Y - tileY; // Позиція всередині клітинки по Y

                    // Визначення, чи є стіна вертикальною (бічною) для коректного накладання текстури
                    bool vertical = fx < 0.1f || fx > 0.9f;

                    // Повернення результату зіткнення
                    return new RaycastHit(position, distance, vertical, map.GetTile(tileX, tileY));
                }
            }
            // Якщо стіну не знайдено на максимальній відстані
            return new RaycastHit(position, MaxDepth, false, 0);
        }

        // Метод малювання міні-карти
        private void DrawMinimap(System.Drawing.Graphics graphics, GameWorld world)
        {
            const int cellSize = 5; // Розмір однієї клітинки на міні-карті
            const int offset = 10; // Відступ карти від краю екрана
            GameMap map = world.Map; // Посилання на карту

            // Створення пензлів для різних елементів карти
            using var wallBrush = new SolidBrush(Color.FromArgb(180, 255, 255, 255)); // Стіни
            using var floorBrush = new SolidBrush(Color.FromArgb(80, 0, 0, 0)); // Підлога
            using var playerBrush = new SolidBrush(Color.Red); // Гравець
            using var enemyBrush = new SolidBrush(Color.Orange); // Вороги

            // Цикли для малювання сітки плиток карти
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    int tile = map.GetTile(x, y);
                    // Малювання підлоги
                    graphics.FillRectangle(floorBrush, offset + x * cellSize, offset + y * cellSize, cellSize, cellSize);

                    // Малювання стін, якщо ID плитки більше нуля
                    if (tile > 0) graphics.FillRectangle(wallBrush, offset + x * cellSize, offset + y * cellSize, cellSize - 1, cellSize - 1);
                }
            }

            // Малювання точки гравця на міні-карті
            float px = offset + world.Player.Position.X * cellSize;
            float py = offset + world.Player.Position.Y * cellSize;
            graphics.FillEllipse(playerBrush, px - 2, py - 2, 4, 4);

            // Малювання точок усіх активних ворогів
            foreach (var entity in world.Entities)
            {
                if (!entity.IsActive) continue;
                graphics.FillEllipse(enemyBrush, offset + entity.Position.X * cellSize - 2, offset + entity.Position.Y * cellSize - 2, 4, 4);
            }
        }
    }
}