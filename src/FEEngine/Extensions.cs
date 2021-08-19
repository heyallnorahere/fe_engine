using System;

namespace FEEngine
{
    public static class Extensions
    {
        public static T VerifyValue<T>(this object? obj, T? value)
        {
            return value ?? throw new NullReferenceException();
        }
        public static bool DerivesFrom(this Type type, Type baseType)
        {
            Type? currentType = type;
            while (currentType != null)
            {
                currentType = currentType.BaseType;
                if (currentType == baseType)
                {
                    return true;
                }
            }
            return false;
        }
    }
}