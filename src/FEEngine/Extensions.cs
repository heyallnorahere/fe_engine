using System;

namespace FEEngine
{
    public static class Extensions
    {
        public static T VerifyValue<T>(this object? obj, T? value)
        {
            return value ?? throw new NullReferenceException();
        }
    }
}