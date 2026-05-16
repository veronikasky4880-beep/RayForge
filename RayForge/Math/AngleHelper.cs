using System;

namespace RayForge.Math
{
    public static class AngleHelper
    {
        public const float Pi = MathF.PI;
        public const float TwoPi = MathF.PI * 2f;
        public const float HalfPi = MathF.PI * 0.5f;
        public static float Normalize(float angle)
        {
            angle %= TwoPi;
            if (angle < 0f)
                angle += TwoPi;

            return angle;
        }
        public static float ToRadians(float degrees)
        {
            return degrees * Pi / 180f;
        }
        public static float ToDegrees(float radians)
        {
            return radians * 180f / Pi;
        }
        public static float Delta(float from, float to)
        {
            float difference = Normalize(to) - Normalize(from);
            if (difference > Pi)
                difference -= TwoPi;
            else if (difference < -Pi)
                difference += TwoPi;

            return difference;
        }
        public static bool IsFacingRight(float angle)
        {
            angle = Normalize(angle);
            return angle < HalfPi || angle > Pi + HalfPi;
        }
        public static bool IsFacingDown(float angle)
        {
            angle = Normalize(angle);
            return angle > 0f && angle < Pi;
        }
    }
}