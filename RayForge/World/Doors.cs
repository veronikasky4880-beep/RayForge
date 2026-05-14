using RayForge.Math;

namespace RayForge.World
{
    // Клас Door представляє двері як ігрову сутність (Entity), яку можна відчинити
    public sealed class Door : Entity
    {
        // Швидкість анімації відчинення дверей
        private const float OpenSpeed = 1.5f;

        // Максимальне значення прогресу відчинення (1 = 100%)
        private const float MaxOpenAmount = 1f;

        // Властивість, яка вказує, чи триває процес відчинення дверей прямо зараз
        public bool IsOpening { get; private set; }

        // Поточне значення того, наскільки двері відчинені (від 0 до 1)
        public float OpenAmount { get; private set; }

        // Конструктор: встановлює початкові координати дверей на карті
        public Door(float x, float y) : base(x, y)
        {
        }

        // Метод для активації процесу відчинення (наприклад, при натисканні клавіші дії)
        public void Open()
        {
            IsOpening = true;
        }

        // Метод оновлення стану дверей кожного кадру
        public override void Update(GameWorld world, float deltaTime)
        {
            // Якщо команда на відчинення не була дана — нічого не робимо
            if (!IsOpening)
                return;

            // Поступове збільшення значення OpenAmount залежно від часу (плавна анімація)
            OpenAmount += OpenSpeed * deltaTime;

            // Перевірка, чи досягли ми максимального відчинення
            if (OpenAmount >= MaxOpenAmount)
            {
                OpenAmount = MaxOpenAmount; // Фіксуємо значення на максимумі
                IsOpening = false;          // Зупиняємо процес анімації

                // Деактивуємо об'єкт (IsActive = false). 
                // У цій логіці двері просто зникають зі світу після відчинення.
                IsActive = false;
            }
        }
    }
}