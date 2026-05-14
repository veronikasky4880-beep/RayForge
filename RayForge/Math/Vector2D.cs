using System;

namespace RayForge.Math
{
    // Структура Vector2D представляє вектор або точку у двовимірному просторі (X, Y)
    public struct Vector2D
    {
        // Координати вектора по горизонталі (X) та вертикалі (Y)
        public float X;
        public float Y;

        // Конструктор для створення вектора із заданими координатами
        public Vector2D(float x, float y)
        {
            X = x;
            Y = y;
        }

        // Обчислення звичайної довжини вектора (за теоремою Піфагора з вирахуванням кореня)
        public readonly float Length =>
            MathF.Sqrt(X * X + Y * Y);

        // Обчислення квадрата довжини вектора (швидше, ніж Length, бо не використовує корінь)
        // Корисно для порівняння відстаней, щоб зекономити ресурси процесора
        public readonly float LengthSquared =>
            X * X + Y * Y;

        // Метод для створення нормалізованого вектора (вектор того ж напрямку, але довжиною 1)
        public readonly Vector2D Normalized()
        {
            float length = Length;

            // Захист від ділення на нуль, якщо вектор має нульову довжину
            if (length <= 0.0001f)
                return new Vector2D(0f, 0f);

            return new Vector2D(X / length, Y / length);
        }

        // Обчислення відстані між поточною точкою та іншою точкою other
        public readonly float DistanceTo(Vector2D other)
        {
            float dx = other.X - X;
            float dy = other.Y - Y;
            return MathF.Sqrt(dx * dx + dy * dy);
        }

        // Обчислення квадрата відстані між точками (оптимізований варіант для порівняння відстаней)
        public readonly float DistanceSquaredTo(Vector2D other)
        {
            float dx = other.X - X;
            float dy = other.Y - Y;
            return dx * dx + dy * dy;
        }

        // Скалярний добуток двох векторів (використовується для розрахунку кутів та освітлення)
        public readonly float Dot(Vector2D other)
        {
            return X * other.X + Y * other.Y;
        }

        // Створення одиничного вектора на основі заданого кута (в радіанах)
        public static Vector2D FromAngle(float angle)
        {
            return new Vector2D(
                MathF.Cos(angle),
                MathF.Sin(angle));
        }

        // Перевантаження оператора додавання двох векторів
        public static Vector2D operator +(Vector2D a, Vector2D b)
            => new(a.X + b.X, a.Y + b.Y);

        // Перевантаження оператора віднімання двох векторів
        public static Vector2D operator -(Vector2D a, Vector2D b)
            => new(a.X - b.X, a.Y - b.Y);

        // Перевантаження оператора множення вектора на число (скаляр)
        public static Vector2D operator *(Vector2D a, float scalar)
            => new(a.X * scalar, a.Y * scalar);

        // Перевантаження оператора ділення вектора на число (скаляр)
        public static Vector2D operator /(Vector2D a, float scalar)
            => new(a.X / scalar, a.Y / scalar);

        // Перетворення вектора в зручний текстовий формат для відладки (наприклад: "(1.50, 2.00)")
        public override readonly string ToString()
            => $"({X:0.00}, {Y:0.00})";
    }
}