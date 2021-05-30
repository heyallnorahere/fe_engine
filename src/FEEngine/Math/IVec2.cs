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
        /// <summary>
        /// The X, or R, field of a vector
        /// </summary>
        T X { get; set; }
        /// <summary>
        /// The Y, or G, field of a vector
        /// </summary>
        T Y { get; set; }
        /// <summary>
        /// Calculates the taxicab length of a vector
        /// </summary>
        /// <returns>The taxicab length</returns>
        T TaxicabLength();
    }
}
