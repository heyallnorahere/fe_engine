using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace FEEngine
{
    [StructLayout(LayoutKind.Sequential)]
    [JsonObject]
    public struct Vector2
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Vector2(int scalar)
        {
            X = Y = scalar;
        }
        public Vector2(int x, int y)
        {
            X = x;
            Y = y;
        }
        public Vector2(Vector2 vector)
        {
            X = vector.X;
            Y = vector.Y;
        }
        public int TaxicabLength
        {
            get
            {
                return Math.Abs(X) + Math.Abs(Y);
            }
        }
        public double Length
        {
            get
            {
                return Math.Sqrt(X * X + Y * Y);
            }
        }
        public Vector2 Clamp(Vector2 min, Vector2 max)
        {
            var newVector = new Vector2(this);
            if (newVector.X < min.X)
            {
                newVector.X = min.X;
            }
            if (newVector.X > max.X)
            {
                newVector.X = max.X;
            }
            if (newVector.Y < min.Y)
            {
                newVector.Y = min.Y;
            }
            if (newVector.Y > max.Y)
            {
                newVector.Y = max.Y;
            }
            return newVector;
        }
        public bool IsOutOfBounds(Vector2 max, Vector2? min = null)
        {
            Vector2 minCoords = min ?? (0, 0);
            if (X >= max.X || Y >= max.Y)
            {
                return true;
            }
            if (X < minCoords.X || Y < minCoords.Y)
            {
                return true;
            }
            return false;
        }
        public static Vector2 operator+(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
        }
        public static Vector2 operator-(Vector2 vector)
        {
            return new Vector2(-vector.X, -vector.Y);
        }
        public static Vector2 operator-(Vector2 v1, Vector2 v2)
        {
            return v1 + -v2;
        }
        public static Vector2 operator*(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X * v2.X, v1.Y * v2.Y);
        }
        public static Vector2 operator*(int scalar, Vector2 vector)
        {
            return vector * scalar;
        }
        public static Vector2 operator*(Vector2 vector, int scalar)
        {
            return vector * new Vector2(scalar);
        }
        public static Vector2 operator/(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X / v2.X, v1.Y / v2.Y);
        }
        public static Vector2 operator/(int scalar, Vector2 vector)
        {
            return new Vector2(scalar) / vector;
        }
        public static Vector2 operator/(Vector2 vector, int scalar)
        {
            return vector / new Vector2(scalar);
        }
        public static bool operator==(Vector2 v1, Vector2 v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y;
        }
        public static bool operator!=(Vector2 v1, Vector2 v2)
        {
            return !(v1 == v2);
        }
        public override bool Equals(object? obj)
        {
            if (obj is Vector2 vector)
            {
                return this == vector;
            }
            else
            {
                return false;
            }
        }
        public override int GetHashCode() => HashCode.Combine(X, Y);
        public static implicit operator Vector2((int x, int y) tuple) => new(tuple.x, tuple.y);
        public static implicit operator Vector2(Microsoft.Xna.Framework.Vector2 vector) => new((int)vector.X, (int)vector.Y);
        public static implicit operator Vector2(Vector2f vector) => new((int)Math.Floor(vector.X), (int)Math.Floor(vector.Y));
    }
    public struct Vector2f
    {
        public float X { get; set; }
        public float Y { get; set; }
        public Vector2f(float scalar)
        {
            X = Y = scalar;
        }
        public Vector2f(float x, float y)
        {
            X = x;
            Y = y;
        }
        public Vector2f(Vector2f vector)
        {
            X = vector.X;
            Y = vector.Y;
        }
        public float TaxicabLength
        {
            get
            {
                return Math.Abs(X) + Math.Abs(Y);
            }
        }
        public double Length
        {
            get
            {
                return Math.Sqrt(X * X + Y * Y);
            }
        }
        public Vector2f Clamp(Vector2f min, Vector2f max)
        {
            var newVector = new Vector2f(this);
            if (newVector.X < min.X)
            {
                newVector.X = min.X;
            }
            if (newVector.X > max.X)
            {
                newVector.X = max.X;
            }
            if (newVector.Y < min.Y)
            {
                newVector.Y = min.Y;
            }
            if (newVector.Y > max.Y)
            {
                newVector.Y = max.Y;
            }
            return newVector;
        }
        public bool IsOutOfBounds(Vector2f max, Vector2f? min = null)
        {
            Vector2f minCoords = min ?? (0f, 0f);
            if (X >= max.X || Y >= max.Y)
            {
                return true;
            }
            if (X < minCoords.X || Y < minCoords.Y)
            {
                return true;
            }
            return false;
        }
        public static Vector2f operator+(Vector2f v1, Vector2f v2)
        {
            return new Vector2f(v1.X + v2.X, v1.Y + v2.Y);
        }
        public static Vector2f operator-(Vector2f vector)
        {
            return new Vector2f(-vector.X, -vector.Y);
        }
        public static Vector2f operator-(Vector2f v1, Vector2f v2)
        {
            return v1 + -v2;
        }
        public static Vector2f operator*(Vector2f v1, Vector2f v2)
        {
            return new Vector2f(v1.X * v2.X, v1.Y * v2.Y);
        }
        public static Vector2f operator*(float scalar, Vector2f vector)
        {
            return vector * scalar;
        }
        public static Vector2f operator*(Vector2f vector, float scalar)
        {
            return vector * new Vector2f(scalar);
        }
        public static Vector2f operator/(Vector2f v1, Vector2f v2)
        {
            return new Vector2f(v1.X / v2.X, v1.Y / v2.Y);
        }
        public static Vector2f operator/(float scalar, Vector2f vector)
        {
            return new Vector2f(scalar) / vector;
        }
        public static Vector2f operator/(Vector2f vector, int scalar)
        {
            return vector / new Vector2f(scalar);
        }
        public static bool operator==(Vector2f v1, Vector2f v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y;
        }
        public static bool operator!=(Vector2f v1, Vector2f v2)
        {
            return !(v1 == v2);
        }
        public override bool Equals(object? obj)
        {
            if (obj is Vector2f vector)
            {
                return this == vector;
            }
            else
            {
                return false;
            }
        }
        public override int GetHashCode() => HashCode.Combine(X, Y);
        public static implicit operator Vector2f((float x, float y) tuple) => new(tuple.x, tuple.y);
        public static implicit operator Vector2f(Microsoft.Xna.Framework.Vector2 vector) => new((int)vector.X, (int)vector.Y);
        public static implicit operator Vector2f(Vector2 vector) => new(vector.X, vector.Y);
    }
}
