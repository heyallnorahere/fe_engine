using System;
using System.Collections;

using System.Runtime.InteropServices;

namespace FEEngine.Math
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct GenericVec2<T> : IVec2<T> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        public T X { get; set; }
        public T Y { get; set; }
        public int Count { get => 2; }
        public T this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                }
                throw new IndexOutOfRangeException();
            }
        }
        public T TaxicabLength()
        {
            return MathUtil.Add<T, T, T>(MathUtil.Abs(X), MathUtil.Abs(Y));
        }
        public IEnumerator GetEnumerator()
        {
            return new VectorEnumerator<T>(this);
        }
        public GenericVec2(T x, T y)
        {
            X = x;
            Y = y;
        }
        public GenericVec2(T scalar)
        {
            X = Y = scalar;
        }
        public GenericVec2(IVec2<T> other)
        {
            X = other.X;
            Y = other.Y;
        }
    }
    /// <summary>
    /// This class exists because generics are quite annoying when it comes to math
    /// </summary>
    public class MathUtil
    {
        public static IVec2<T> ClampVector<T>(IVec2<T> inputVector, IVec2<T> min, IVec2<T> max) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
        {
            IVec2<T> copy = new GenericVec2<T>(inputVector);
            if (copy.X.CompareTo(min.X) < 0)
            {
                copy.X = min.X;
            }
            if (copy.X.CompareTo(max.X) > 0)
            {
                copy.X = max.X;
            }
            if (copy.Y.CompareTo(min.Y) < 0)
            {
                copy.Y = min.Y;
            }
            if (copy.Y.CompareTo(max.Y) > 0)
            {
                copy.Y = max.Y;
            }
            return copy;
        }
        public static bool AreVectorsEqual<T>(IVec2<T> v1, IVec2<T> v2) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
        {
            return v1.X.Equals(v2.X) && v1.Y.Equals(v2.Y);
        }
        public static bool IsVectorOutOfBounds<T>(IVec2<T> vector, IVec2<T> boundsMax, IVec2<T> boundsMin = null) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
        {
            if (vector.X.CompareTo(boundsMax.X) >= 0)
            {
                return true;
            }
            if (vector.Y.CompareTo(boundsMax.Y) >= 0)
            {
                return true;
            }
            T zero = (T)Convert.ChangeType(0, typeof(T));
            if (vector.X.CompareTo(boundsMin?.X ?? zero) < 0)
            {
                return true;
            }
            if (vector.Y.CompareTo(boundsMin?.Y ?? zero) < 0)
            {
                return true;
            }
            return false;
        }
        public static IVec2<T> AddVectors<T>(IVec2<T> v1, IVec2<T> v2) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
        {
            T x = Add<T, T, T>(v1.X, v2.X);
            T y = Add<T, T, T>(v1.Y, v2.Y);
            return new GenericVec2<T>(x, y);
        }
        public static void AddVectors<T>(ref IVec2<T> v1, IVec2<T> v2) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
        {
            v1 = AddVectors(v1, v2);
        }
        public static IVec2<T> SubVectors<T>(IVec2<T> v1, IVec2<T> v2) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
        {
            return new GenericVec2<T>(Sub<T, T, T>(v1.X, v2.X), Sub<T, T, T>(v1.Y, v2.Y));
        }
        public static void SubVectors<T>(ref IVec2<T> v1, IVec2<T> v2) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
        {
            v1 = SubVectors(v1, v2);
        }
        public static T Add<T, P1, P2>(P1 v1, P2 v2) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
        {
            return (T)Convert.ChangeType(Convert.ToDouble(v1) + Convert.ToDouble(v2), typeof(T));
        }
        public static T Sub<T, P1, P2>(P1 v1, P2 v2) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
        {
            return (T)Convert.ChangeType(Convert.ToDouble(v1) - Convert.ToDouble(v2), typeof(T));
        }
        public static T Mul<T, P1, P2>(P1 v1, P2 v2) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
        {
            return (T)Convert.ChangeType(Convert.ToDouble(v1) * Convert.ToDouble(v2), typeof(T));
        }
        public static T Abs<T>(T value) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
        {
            if (value.CompareTo((T)Convert.ChangeType(0, typeof(T))) < 0)
            {
                value = Mul<T, T, int>(value, -1);
            }
            return value;
        }
    }
}
