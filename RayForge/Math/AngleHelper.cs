using System;

namespace RayForge.Math
{
    // Статичний клас AngleHelper надає допоміжні методи для роботи з кутами та тригонометрією
    public static class AngleHelper
    {
        // Константа числа Пі (приблизно 3.14159)
        public const float Pi = MathF.PI;

        // Константа повного кола в радіанах (360 градусів)
        public const float TwoPi = MathF.PI * 2f;

        // Константа чверті кола або 90 градусів у радіанах
        public const float HalfPi = MathF.PI * 0.5f;

        // Метод для нормалізації кута, щоб він завжди залишався в межах від 0 до 2*Pi
        public static float Normalize(float angle)
        {
            // Обчислення залишку від ділення, щоб "скинути" зайві оберти
            angle %= TwoPi;

            // Якщо кут від'ємний, додаємо повне коло, щоб зробити його додатним
            if (angle < 0f)
                angle += TwoPi;

            return angle;
        }

        // Переведення значення з градусів у радіани
        public static float ToRadians(float degrees)
        {
            return degrees * Pi / 180f;
        }

        // Переведення значення з радіан у градуси
        public static float ToDegrees(float radians)
        {
            return radians * 180f / Pi;
        }

        // Обчислення найкоротшої різниці (дельти) між двома кутами
        public static float Delta(float from, float to)
        {
            // Різниця між нормалізованими кутами
            float difference = Normalize(to) - Normalize(from);

            // Якщо різниця більша за півколо, коригуємо її для пошуку найкоротшого шляху
            if (difference > Pi)
                difference -= TwoPi;
            // Якщо різниця менша за мінус півколо, додаємо повне коло
            else if (difference < -Pi)
                difference += TwoPi;

            return difference;
        }

        // Перевірка, чи спрямований кут праворуч відносно осі X
        public static bool IsFacingRight(float angle)
        {
            angle = Normalize(angle);
            // Кут спрямований праворуч, якщо він у першій або четвертій чверті кола
            return angle < HalfPi || angle > Pi + HalfPi;
        }

        // Перевірка, чи спрямований кут донизу (у контексті екранних координат, де Y зростає вниз)
        public static bool IsFacingDown(float angle)
        {
            angle = Normalize(angle);
            // Кут спрямований вниз, якщо він знаходиться між 0 та Pi радіан
            return angle > 0f && angle < Pi;
        }
    }
}