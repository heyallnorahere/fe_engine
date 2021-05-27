using System;
using Newtonsoft.Json;

namespace FEEngine.Math
{
    [JsonObject]
    public interface IVec2<T> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        T X { get; set; }
        T Y { get; set; }
        T TaxicabLength();
    }
}
