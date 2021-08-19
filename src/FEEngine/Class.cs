using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FEEngine
{
    public enum MovementType
    {
        /// <summary>
        /// On foot
        /// </summary>
        Infantry,
        /// <summary>
        /// On a horse
        /// </summary>
        Cavalry,
        /// <summary>
        /// On a flying creature (pegasus, wyvern, etc.)
        /// </summary>
        Flying
    }
    [JsonObject]
    public struct MovementProperties
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public MovementType MovementType { get; set; }
        public bool IsArmored { get; set; }
    }
    /// <summary>
    /// A <see cref="Unit"/>'s categorization. Pre-written classes reside in the <see cref="Classes"/> namespace.
    /// </summary>
    [JsonObject]
    public abstract class Class
    {
        public static Class GetClass(string name)
        {
            Type? classType = Type.GetType(name);
            if (!(classType?.DerivesFrom(typeof(Class)) ?? false))
            {
                throw new ArgumentException("The given type does not derive from Class!");
            }
            ConstructorInfo? constructor = classType?.GetConstructor(Array.Empty<Type>());
            object createdObject = Extensions.VerifyValue(null, constructor?.Invoke(Array.Empty<object>()));
            return (Class)createdObject;
        }
        public static Class GetClass(Type type)
        {
            return GetClass(Extensions.VerifyValue(null, type.AssemblyQualifiedName));
        }
        public static Class GetClass<T>() where T : Class, new()
        {
            return GetClass(typeof(T));
        }
        public Class()
        {
            ClassSkills = new Skill?[3] { null, null, null };
        }
        /// <summary>
        /// Whether the <see cref="Unit"/> is mounted or not
        /// </summary>
        public abstract MovementProperties MovementProperties { get; }
        /// <summary>
        /// The name of the <see cref="Class"/>
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// The stat boosts that users of this class will receive
        /// </summary>
        public abstract Unit.UnitStats StatBoosts { get; }
        /// <summary>
        /// An array of 3 <see cref="Skill"/>s that will have effect on the user of this class
        /// </summary>
        public Skill?[] ClassSkills { get; private set; }
    }
}
