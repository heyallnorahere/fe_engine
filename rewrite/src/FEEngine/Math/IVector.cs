using System;
using System.Collections;

namespace FEEngine.Math
{
    public interface IVector<T> : IEnumerable where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        int Count { get; }
        T this[int index] { get; }
    }
    public struct VectorEnumerator<T> : IEnumerator where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        public bool MoveNext()
        {
            if (mPosition < mParent.Count - 1)
            {
                mPosition++;
                return true;
            }
            return false;
        }
        public void Reset()
        {
            mPosition = -1;
        }
        public object Current
        {
            get
            {
                return mParent[mPosition];
            }
        }
        public VectorEnumerator(IVector<T> parent)
        {
            mParent = parent;
            mPosition = -1;
        }
        private int mPosition;
        private IVector<T> mParent;
    }
}
