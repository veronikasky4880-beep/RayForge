using System;
namespace RayForge.Math
{
    public struct Vector2D
    {
        public float X;
        public float Y;
        public Vector2D(float x, float y)
        {
            X = x;
            Y = y;
        }
        public readonly float Length =>
            MathF.Sqrt(X * X + Y * Y);
        public readonly float LengthSquared =>
            X * X + Y * Y;
        public readonly Vector2D Normalized()
        {
            float length = Length;
            if (length <= 0.0001f)
                return new Vector2D(0f, 0f);

            return new Vector2D(X / length, Y / length);
        }
        public readonly float DistanceTo(Vector2D other)
        {
            float dx = other.X - X;
            float dy = other.Y - Y;
            return MathF.Sqrt(dx * dx + dy * dy);
        }
        public readonly float DistanceSquaredTo(Vector2D other)
        {
            float dx = other.X - X;
            float dy = other.Y - Y;
            return dx * dx + dy * dy;
        }
        public readonly float Dot(Vector2D other)
        {
            return X * other.X + Y * other.Y;
        }
        public static Vector2D FromAngle(float angle)
        {
            return new Vector2D(
                MathF.Cos(angle),
                MathF.Sin(angle));
        }
        public static Vector2D operator +(Vector2D a, Vector2D b)
            => new(a.X + b.X, a.Y + b.Y);
        public static Vector2D operator -(Vector2D a, Vector2D b)
            => new(a.X - b.X, a.Y - b.Y);
        public static Vector2D operator *(Vector2D a, float scalar)
            => new(a.X * scalar, a.Y * scalar);
        public static Vector2D operator /(Vector2D a, float scalar)
            => new(a.X / scalar, a.Y / scalar);
        public override readonly string ToString()
            => $"({X:0.00}, {Y:0.00})";
    }
}