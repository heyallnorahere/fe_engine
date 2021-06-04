using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FEEngine
{
    public enum MovementType
    {
        Foot,
        Mounted,
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
        public Class()
        {
            ClassSkills = new Skill[3] { null, null, null };
        }
        /// <summary>
        /// Whether the <see cref="Unit"/> is mounted or not
        /// </summary>
        public abstract MovementProperties MovementProperties { get; }
        /// <summary>
        /// The name of the <see cref="Class"/>
        /// </summary>
        public abstract string Name { get; }
        public abstract Unit.UnitStats StatBoosts { get; }
        public Skill[] ClassSkills { get; private set; }
    }
}
