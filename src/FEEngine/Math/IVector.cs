using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FEEngine.Math
{
    /// <summary>
    /// An interface for a generic vector
    /// </summary>
    /// <typeparam name="T">The type of number to store</typeparam>
    public interface IVector<T> : IEnumerable<T> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        [JsonIgnore]
        int Count { get; }
        T this[int index] { get; }
    }
    public struct VectorEnumerator<T> : IEnumerator<T> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
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
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public T Current
        {
            get
            {
                return mParent[mPosition];
            }
        }
        object IEnumerator.Current
        {
            get
            {
                return Current;
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
