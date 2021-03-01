using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace FEEngine.Math
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vec2<T>
    {
        public T X, Y;
        public Vec2(T scalar)
        {
            this.X = this.Y = scalar;
        }
        public Vec2(T x, T y)
        {
            this.X = x;
            this.Y = y;
        }
        public void Clamp(Vec2<T> min, Vec2<T> max)
        {
            if (Convert.ToDouble(this.X) < Convert.ToDouble(min.X))
            {
                this.X = min.X;
            }
            if (Convert.ToDouble(this.X) > Convert.ToDouble(max.X))
            {
                this.X = max.X;
            }
            if (Convert.ToDouble(this.Y) < Convert.ToDouble(min.Y))
            {
                this.Y = min.Y;
            }
            if (Convert.ToDouble(this.Y) > Convert.ToDouble(max.Y))
            {
                this.Y = max.Y;
            }
        }
        public Vec2<double> Normalize()
        {
            double x = Convert.ToDouble(this.X);
            double y = Convert.ToDouble(this.Y);
            double length = Convert.ToDouble(this.TaxicabLength());
            return new Vec2<double>(x / length, y / length);
        }
        public T TaxicabLength()
        {
            T x = (T)Convert.ChangeType(System.Math.Abs(Convert.ToDouble(this.X)), typeof(T));
            T y = (T)Convert.ChangeType(System.Math.Abs(Convert.ToDouble(this.Y)), typeof(T));
            return (T)Convert.ChangeType(Convert.ToDouble(x) + Convert.ToDouble(y), typeof(T));
        }
        public Vec2<U> ConvertTo<U>()
        {
            Vec2<U> vec = new Vec2<U>();
            vec.X = (U)Convert.ChangeType(this.X, typeof(U));
            vec.Y = (U)Convert.ChangeType(this.Y, typeof(U));
            return vec;
        }
        public static Vec2<T> operator-(Vec2<T> vector)
        {
            double x = -Convert.ToDouble(vector.X);
            double y = -Convert.ToDouble(vector.Y);
            return new Vec2<T>((T)Convert.ChangeType(x, typeof(T)), (T)Convert.ChangeType(y, typeof(T)));
        }
        public static Vec2<T> operator+(Vec2<T> vec1, Vec2<T> vec2)
        {
            return new Vec2<T>((T)Convert.ChangeType(Convert.ToDouble(vec1.X) + Convert.ToDouble(vec2.X), typeof(T)), (T)Convert.ChangeType(Convert.ToDouble(vec1.Y) + Convert.ToDouble(vec2.Y), typeof(T)));
        }
        public static Vec2<T> operator-(Vec2<T> vec1, Vec2<T> vec2)
        {
            return vec1 + (-vec2);
        }
        public static Vec2<T> operator*(Vec2<T> vec, T scalar)
        {
            return new Vec2<T>((T)Convert.ChangeType(Convert.ToDouble(vec.X) * Convert.ToDouble(scalar), typeof(T)), (T)Convert.ChangeType(Convert.ToDouble(vec.Y) * Convert.ToDouble(scalar), typeof(T)));
        }
        public static Vec2<T> operator/(Vec2<T> vec, T scalar)
        {
            return new Vec2<T>((T)Convert.ChangeType(Convert.ToDouble(vec.X) / Convert.ToDouble(scalar), typeof(T)), (T)Convert.ChangeType(Convert.ToDouble(vec.Y) / Convert.ToDouble(scalar), typeof(T)));
        }
    }
}
