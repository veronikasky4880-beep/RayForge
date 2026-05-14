using System;
using System.Collections.Generic;
using System.Drawing;
using RayForge.Math;
using RayForge.World;

namespace RayForge.Graphics
{
    // Клас SpriteRenderer відповідає за малювання об'єктів (ворогів, предметів) у 3D-просторі
    public sealed class SpriteRenderer
    {
        // Константа кута огляду (повинна збігатися з налаштуваннями Raycaster)
        private const float FieldOfView = MathF.PI / 3f;

        // Посилання на менеджер текстур для отримання зображень об'єктів
        private readonly TextureManager _textures;

        // Конструктор, що приймає менеджер текстур
        public SpriteRenderer(TextureManager textures)
        {
            _textures = textures;
        }

        // Основний метод рендерингу всіх спрайтів
        public void Render(
            System.Drawing.Graphics graphics,
            GameWorld world,
            IReadOnlyList<Entity> entities,
            Size viewport,
            float[] zBuffer)
        {
            // Створюємо список об'єктів для сортування (не змінюючи оригінальний список)
            var sortedEntities = new List<Entity>(entities);

            // Сортування об'єктів від найвіддаленішого до найближчого (алгоритм художника)
            // Це необхідно, щоб ближні об'єкти малювалися поверх дальніх
            sortedEntities.Sort((a, b) =>
            {
                // Обчислення квадрата відстані (швидше, ніж звичайне вирахування відстані з коренем)
                float distA = (a.Position - world.Player.Position).LengthSquared;
                float distB = (b.Position - world.Player.Position).LengthSquared;

                // Порівняння для сортування за спаданням відстані
                return distB.CompareTo(distA);
            });

            // Прохід по кожному відсортованому об'єкту
            foreach (Entity entity in sortedEntities)
            {
                // Якщо об'єкт зібраний або неактивний — пропускаємо його
                if (!entity.IsActive) continue;

                // Виклик методу малювання конкретної сутності
                DrawEntity(graphics, world, entity, viewport, zBuffer);
            }
        }

        // Метод для розрахунку та малювання одного конкретного спрайта
        private void DrawEntity(System.Drawing.Graphics graphics, GameWorld world, Entity entity, Size viewport, float[] zBuffer)
        {
            // Вектор від гравця до об'єкта
            Vector2D delta = entity.Position - world.Player.Position;

            // Реальна відстань до об'єкта
            float distance = delta.Length;

            // Відсікання об'єктів, які знаходяться практично впритул до гравця (захист від помилок)
            if (distance < 0.1f) return;

            // Обчислення кута напрямку на об'єкт відносно світу
            float angleToEntity = MathF.Atan2(delta.Y, delta.X);

            // Кут об'єкта відносно напрямку погляду гравця
            float relativeAngle = AngleHelper.Delta(world.Player.Angle, angleToEntity);

            // Перевірка, чи знаходиться об'єкт у полі зору (з невеликим запасом 1.5x)
            if (MathF.Abs(relativeAngle) > FieldOfView * 1.5f) return;

            // Корекція відстані для усунення спотворень по краях екрана
            float correctedDistance = distance * MathF.Cos(relativeAngle);
            if (correctedDistance < 0.1f) correctedDistance = 0.1f;

            // Розрахунок висоти спрайта на екрані (чим далі об'єкт, тим він менший)
            // Коефіцієнт 0.7f підібраний для того, щоб спрайт не виглядав занадто високим
            float spriteHeight = (viewport.Height / correctedDistance) * 0.7f;

            // Спрайт зазвичай квадратний, тому ширина дорівнює висоті
            float spriteWidth = spriteHeight;

            // Позиція центру спрайта на екрані по горизонталі (X)
            float screenX = (relativeAngle / FieldOfView + 0.5f) * viewport.Width;

            // Центрування спрайта по вертикалі (Y) — він малюється посередині лінії горизонту
            float top = (viewport.Height - spriteHeight) * 0.5f;

            // Отримання текстури за її ідентифікатором
            Bitmap texture = _textures.GetTexture(entity.TextureId);

            // Визначення початкової та кінцевої точок малювання по горизонталі
            int startX = (int)(screenX - spriteWidth / 2);
            int endX = (int)(screenX + spriteWidth / 2);

            // Обмеження малювання межами екрана (лівий край)
            int drawStartX = startX;
            if (drawStartX < 0) drawStartX = 0;

            // Обмеження малювання межами екрана (правий край)
            int drawEndX = endX;
            if (drawEndX > viewport.Width) drawEndX = viewport.Width;

            // Попіксельний цикл малювання вертикальних смужок спрайта
            for (int x = drawStartX; x < drawEndX; x++)
            {
                // Перевірка Z-буфера (чи не закрита ця частина спрайта стіною)
                if (x >= 0 && x < zBuffer.Length)
                {
                    // Якщо відстань у буфері менша за відстань до спрайта — стіна ближче, спрайт не малюємо
                    if (zBuffer[x] < correctedDistance - 0.1f) continue;
                }

                // Визначення координати X на самій текстурі (від 0 до 1)
                float texXPercent = (float)(x - startX) / spriteWidth;
                int texX = (int)(texXPercent * texture.Width);

                // Захист від виходу за межі ширини текстури
                if (texX < 0 || texX >= texture.Width) continue;

                // Розрахунок фінальної висоти для малювання (з обмеженням для стабільності GDI+)
                int h = (int)spriteHeight;
                int limit = viewport.Height * 2;
                int finalHeight = h > limit ? limit : h;

                // Верхня точка, з якої починається малювання стовпця
                int renderTop = (int)top;

                // Якщо висота позитивна — малюємо вертикальну лінію товщиною в 1 піксель
                if (finalHeight > 0)
                {
                    graphics.DrawImage(texture,
                        new Rectangle(x, renderTop, 1, finalHeight), // Куди малюємо на екрані
                        new Rectangle(texX, 0, 1, texture.Height),   // Яку частину текстури беремо
                        GraphicsUnit.Pixel);
                }
            }
        }
    }
}