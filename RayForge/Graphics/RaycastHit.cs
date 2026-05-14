using RayForge.Math;

namespace RayForge.Graphics
{
    // Структура RaycastHit зберігає інформацію про точку зіткнення променя зі стіною
    public readonly struct RaycastHit
    {
        // Точні координати (X, Y) у світі, де промінь влучив у перешкоду
        public Vector2D Position { get; }

        // Відстань від гравця (початку променя) до точки зіткнення
        public float Distance { get; }

        // Прапорець: true, якщо промінь влучив у вертикальну сторону стіни (збоку), 
        // false — якщо в горизонтальну (зверху/знизу). Використовується для розрахунку освітлення та текстур.
        public bool HitVerticalWall { get; }

        // Ідентифікатор типу плитки (стіни), у яку влучив промінь (для вибору потрібної текстури)
        public int TileId { get; }

        // Конструктор для створення об'єкта з результатами трасування
        public RaycastHit(
            Vector2D position,
            float distance,
            bool hitVerticalWall,
            int tileId)
        {
            Position = position;       // Встановлення позиції влучання
            Distance = distance;       // Встановлення пройденої відстані
            HitVerticalWall = hitVerticalWall; // Запис типу поверхні
            TileId = tileId;           // Запис номера текстури
        }
    }
}