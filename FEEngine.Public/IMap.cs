/*
   Copyright 2022 Nora Beda

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System.Collections.Generic;

namespace FEEngine
{
    public struct MapDesc : ICreationDesc
    {
        public MapDesc()
        {
            Size = (0, 0);
            Name = null;
        }

        /// <summary>
        /// The size of this map. Must be at least 1x1.
        /// </summary>
        public Vector Size;

        /// <summary>
        /// The name of this map. Can be null.
        /// </summary>
        public string? Name;

        public bool Verify() => Size.X >= 1 && Size.Y >= 1;
        public ICreationDesc Clone() => (ICreationDesc)MemberwiseClone();
    }
    
    /// <summary>
    /// A map is a grid of <see cref="IUnit"/>s.
    /// </summary>
    [FactoryInterface]
    public interface IMap
    {
        /// <summary>
        /// Checks if a given point is out of bounds.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <returns>See above.</returns>
        public bool IsOutOfBounds(Vector point);

        /// <summary>
        /// The size of this map.
        /// </summary>
        public Vector Size { get; }

        /// <summary>
        /// The name of this map.
        /// </summary>
        public string? Name { get; }

        /// <summary>
        /// Adds a unit to this map.
        /// </summary>
        /// <param name="unit">The unit to add.</param>
        /// <returns>The index of the added unit. Returns -1 if the function failed.</returns>
        public int AddUnit(IUnit unit);

        /// <summary>
        /// The units contained by this map.
        /// </summary>
        public IReadOnlyList<IUnit> Units { get; }

        /// <summary>
        /// Pushes an action to be run when the map is flushed.
        /// </summary>
        /// <param name="action">The action to push.</param>
        /// <returns>The index of the pushed action.</returns>
        public int PushAction(Action action);

        /// <summary>
        /// Flushes the map and runs every action pushed by <see cref="PushAction(Action)"/>.
        /// </summary>
        /// <returns>If all actions succeeded. If no actions were pushed, returns true.</returns>
        public bool Flush();
    }
}
