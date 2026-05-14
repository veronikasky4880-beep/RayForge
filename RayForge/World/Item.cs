using System;
using RayForge.Math;

namespace RayForge.World
{
    // Клас Item представляє предмети, які гравець може збирати на рівні (аптечки, патрони, золото)
    public class Item : Entity
    {
        // Властивість, яка вказує, чи був предмет уже підібраний гравцем
        public bool IsCollected { get; private set; }

        // Конструктор: передає початкові координати у базовий клас Entity та активує предмет
        public Item(float x, float y) : base(x, y)
        {
            // Встановлюємо стан активності, щоб предмет оновлювався та відображався
            IsActive = true;
        }

        // Метод оновлення стану предмета кожного кадру
        public override void Update(GameWorld world, float deltaTime)
        {
            // Якщо предмет уже зібраний або неактивний — нічого не робимо
            if (IsCollected || !IsActive) return;

            // Обчислюємо відстань між поточною позицією предмета та гравцем
            float dist = (Position - world.Player.Position).Length;

            // Перевірка на зіткнення: якщо гравець підійшов ближче ніж на 0.6 метра
            if (dist < 0.6f)
            {
                // Позначаємо, що предмет зібрано
                IsCollected = true;

                // Деактивуємо об'єкт, щоб він зник із рендерингу та перестав оновлюватися
                IsActive = false;

                // Виводимо повідомлення в консоль для відладки
                Console.WriteLine("Предмет зібрано!");
            }
        }
    }
}