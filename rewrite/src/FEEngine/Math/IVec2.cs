using System;
using System.Collections;
using Newtonsoft.Json;

namespace FEEngine.Math
{
    /// <summary>
    /// This interface exists for users of this engine to create their own vector implementations
    /// </summary>
    /// <typeparam name="T">The type of number that this vector stores</typeparam>
    [JsonObject]
    public interface IVec2<T> : IVector<T> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        T X { get; set; }
        T Y { get; set; }
        T TaxicabLength();
    }
}
