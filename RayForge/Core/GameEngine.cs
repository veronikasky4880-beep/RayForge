using System; // Підключення базового простору імен .NET (типи, математичні функції, події тощо)
using System.Diagnostics; // Підключення Stopwatch для вимірювання часу
using System.Drawing; // Підключення графіки (Bitmap, Graphics, Color)
using System.Windows.Forms; // Підключення Windows Forms (Form, Timer, MessageBox)
using RayForge.Graphics; // Підключення графічних компонентів рушія RayForge
using RayForge.World; // Підключення логіки світу (карта, сутності, гравець)
using RayForge.Math; // Підключення математичних структур (Vector2D, Angle тощо)

namespace RayForge.Core // Простір імен ядра гри
{
    public sealed class GameEngine : IDisposable // Основний рушій гри, реалізує IDisposable для коректного звільнення ресурсів
    {
        private readonly Form _window; // Посилання на головне вікно гри
        private readonly System.Windows.Forms.Timer _gameTimer; // Таймер для оновлення гри (ігровий цикл)
        private readonly Stopwatch _stopwatch; // Таймер високої точності для обчислення deltaTime

        private readonly InputController _input; // Контролер введення
        private readonly GameWorld _world; // Об'єкт світу гри (карта, гравець, сутності)
        private readonly Raycaster _raycaster; // Рейкастер — відповідає за рендер стін
        private readonly SpriteRenderer _spriteRenderer; // Рендер спрайтів (ворогів, предметів)

        private long _lastTicks; // Зберігання попереднього часу для розрахунку deltaTime
        private bool _isGameOver = false; // Прапорець завершення гри

        public GameEngine(Form window) // Конструктор рушія, приймає головне вікно
        {
            _window = window; // Збереження посилання на вікно

            _input = new InputController(window); // Ініціалізація системи введення
            _world = new GameWorld(); // Створення ігрового світу
            _raycaster = new Raycaster(); // Створення рендерера стін
            _spriteRenderer = new SpriteRenderer(_raycaster.Textures); // Створення рендерера спрайтів з доступом до текстур

            // Завантаження текстури монети (ID 7)
            _raycaster.Textures.LoadTexture(7, "assets/item.png"); // Завантаження зображення предмета
            // Завантаження текстури ворога
            _raycaster.Textures.LoadTexture(10, "assets/enemy.png"); // Завантаження текстури ворога

            // Створення ворога у світі
            _world.Entities.Add(new Enemy(5.5f, 5.5f) { TextureId = 10 }); // Додавання ворога з текстурою

            // Створення трьох монет у різних координатах
            _world.Entities.Add(new Item(3.5f, 1.5f) { TextureId = 7 }); // Перша монета
            _world.Entities.Add(new Item(10.5f, 1.5f) { TextureId = 7 }); // Друга монета
            _world.Entities.Add(new Item(6.5f, 5.5f) { TextureId = 7 }); // Третя монета

            _stopwatch = Stopwatch.StartNew(); // Запуск точного таймера
            _lastTicks = _stopwatch.ElapsedTicks; // Збереження початкового часу

            _gameTimer = new System.Windows.Forms.Timer // Створення таймера гри
            {
                Interval = 16 // Інтервал ~16 мс (~60 FPS)
            };

            _gameTimer.Tick += UpdateFrame; // Прив’язка методу оновлення кадру до таймера
            _gameTimer.Start(); // Запуск ігрового циклу
        }

        private void UpdateFrame(object? sender, EventArgs e) // Метод оновлення одного кадру
        {
            if (_isGameOver) return; // Якщо гра завершена — нічого не оновлюємо

            long currentTicks = _stopwatch.ElapsedTicks; // Поточний час у тиках
            float deltaTime = (currentTicks - _lastTicks) / (float)Stopwatch.Frequency; // Розрахунок часу між кадрами в секундах
            _lastTicks = currentTicks; // Оновлення попереднього часу

            // Захист від занадто великого deltaTime
            if (deltaTime > 0.05f) deltaTime = 0.05f; // Обмеження, щоб уникнути телепортації

            _world.Update(_input, deltaTime); // Оновлення логіки світу (рух, взаємодія)

            // --- Логіка перевірки зібраних предметів ---
            int itemsRemaining = 0; // Лічильник активних монет

            foreach (var entity in _world.Entities) // Перебір усіх сутностей у світі
            {
                if (entity is Item && entity.IsActive) // Якщо це предмет і він ще не зібраний
                {
                    itemsRemaining++; // Збільшуємо кількість залишених монет
                }
            }

            _window.Text = $"RayForge - ЗІБРАТИ МОНЕТКИ: {itemsRemaining}"; // Оновлення заголовка вікна

            // Якщо всі монети зібрані
            if (itemsRemaining == 0)
            {
                _isGameOver = true; // Позначаємо завершення гри
                _gameTimer.Stop(); // Зупиняємо таймер гри

                _window.Text = "ПЕРЕМОГА! Золото ЗІБРАНЕ!"; // Повідомлення у заголовку
                _window.Invalidate(); // Примусове перемальовування

                MessageBox.Show("ВИ ЗІБРАЛИ ВСІ 3 МОНЕТКИ І ВИГРАЛИ!", "УСПІХ!"); // Вікно перемоги
                return; // Вихід з методу
            }

            _window.Invalidate(); // Перемальовування вікна
        }

        public void Render(System.Drawing.Graphics graphics) // Метод рендерингу кадру
        {
            _raycaster.Render(graphics, _world, _window.ClientSize); // Спочатку малюємо стіни

            _spriteRenderer.Render( // Потім малюємо спрайти поверх
                graphics,
                _world,
                _world.Entities,
                _window.ClientSize,
                _raycaster.ZBuffer); // Використання Z-буфера(масив де записується відстань до стіни для кожного стовпця пікселів екрана) для правильного перекриття
        }

        public void Dispose() // Метод звільнення ресурсів
        {
            _gameTimer.Stop(); // Зупинка таймера
            _gameTimer.Dispose(); // Звільнення таймера
            _input.Dispose(); // Звільнення ресурсів введення
        }
    }
}