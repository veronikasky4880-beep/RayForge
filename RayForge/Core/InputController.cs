using System; // Підключення базових типів .NET
using System.Collections.Generic; // Підключення колекцій (HashSet)
using System.Windows.Forms; // Підключення типів для роботи з формою та клавішами

namespace RayForge.Core // Простір імен ядра гри
{
    public sealed class InputController : IDisposable // Клас для обробки вводу; реалізує IDisposable для відписки від подій
    {
        private readonly HashSet<Keys> _pressedKeys = new(); // Множина натиснутих клавіш (зберігає тільки унікальні значення)
        private readonly Form _window; // Посилання на головне вікно для підписки на події клавіатури

        public InputController(Form window) // Конструктор, який приймає форму
        {
            _window = window; // Збереження посилання на вікно

            _window.KeyDown += OnKeyDown; // Підписка на подію натискання клавіші
            _window.KeyUp += OnKeyUp; // Підписка на подію відпускання клавіші
        }

        public bool IsKeyDown(Keys key) // Метод перевірки: чи натиснута конкретна клавіша
        {
            return _pressedKeys.Contains(key); // Повертає true, якщо клавіша є у множині натиснутих
        }

        private void OnKeyDown(object? sender, KeyEventArgs e) // Обробник події натискання клавіші
        {
            _pressedKeys.Add(e.KeyCode); // Додає клавішу до множини натиснутих
        }

        private void OnKeyUp(object? sender, KeyEventArgs e) // Обробник події відпускання клавіші
        {
            _pressedKeys.Remove(e.KeyCode); // Видаляє клавішу з множини натиснутих
        }

        public void Dispose() // Метод звільнення ресурсів
        {
            _window.KeyDown -= OnKeyDown; // Відписка від події натискання
            _window.KeyUp -= OnKeyUp; // Відписка від події відпускання
        }
    }
}