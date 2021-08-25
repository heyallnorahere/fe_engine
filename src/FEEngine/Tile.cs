using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FEEngine
{
    public class Tile : RegisteredObjectBase<Tile>
    {
        public enum MovementLimitEnum
        {
            /// <summary>
            /// No units can pass through this tile
            /// </summary>
            None,
            /// <summary>
            /// Flying units can pass through this tile
            /// </summary>
            Flying,
            /// <summary>
            /// All units can pass through this tile
            /// </summary>
            All
        }
        public Tile(Vector2 position, MovementLimitEnum movementLimit)
        {
            Position = position;
            MovementLimit = movementLimit;
        }
        /// <summary>
        /// Checks if the specified <see cref="Unit"/> can move on or past this tile
        /// </summary>
        /// <param name="unit">The unit to check</param>
        /// <returns>See summary</returns>
        public bool CanPass(Unit unit)
        {
            MovementType movementType = unit.Class.MovementProperties.MovementType;
            return MovementLimit switch
            {
                MovementLimitEnum.Flying => movementType == MovementType.Flying,
                MovementLimitEnum.All => true,
                _ => false
            };
        }
        /// <summary>
        /// The position of this tile
        /// </summary>
        public Vector2 Position { get; private set; }
        /// <summary>
        /// What kinds of units can move on or past this tile
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public MovementLimitEnum MovementLimit { get; private set; }
    }
}
