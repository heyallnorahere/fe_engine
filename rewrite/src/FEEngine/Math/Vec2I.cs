using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FEEngine.Math
{
    /// <summary>
    /// A default <see cref="int"/> implementation for <see cref="IVec2{T}"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Vec2I : IVec2<int>
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Count { get => 2; }
        public int this[int index]
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
        public int TaxicabLength()
        {
            return System.Math.Abs(X) + System.Math.Abs(Y);
        }
        public IEnumerator<int> GetEnumerator()
        {
            return new VectorEnumerator<int>(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public Vec2I(int x, int y)
        {
            X = x;
            Y = y;
        }
        public Vec2I(int scalar)
        {
            X = Y = scalar;
        }
        public Vec2I(IVec2<int> other)
        {
            X = other.X;
            Y = other.Y;
        }
    }
}
