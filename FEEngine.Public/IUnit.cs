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
    public struct UnitDesc : ICreationDesc
    {
        /// <summary>
        /// The name of this unit. Can be null.
        /// </summary>
        public string? Name = null;

        /// <summary>
        /// The position at which this unit starts.
        /// </summary>
        public Vector StartingPosition = (0, 0);

        public bool Verify() => true;
    }

    /// <summary>
    /// Represents a unit. Owned by a <see cref="IMap">map</see>.
    /// </summary>
    [FactoryInterface]
    public interface IUnit
    {
        /// <summary>
        /// The name of this unit.
        /// </summary>
        public string? Name { get; }

        /// <summary>
        /// The position of this unit.
        /// </summary>
        public Vector Position { get; set; }

        /// <summary>
        /// The map that contains this unit.
        /// </summary>
        public IMap? Map { get; }

        /// <summary>
        /// Sets the map that contains this unit.
        /// </summary>
        /// <param name="map">The map to set.</param>
        public void SetMap(IMap map);

        /// <summary>
        /// Adds an action to run when the map is flushed.
        /// </summary>
        /// <param name="action">The action to add.</param>
        public bool AddAction(Action action);

        /// <summary>
        /// Resets <see cref="ActionIndices"/>.
        /// </summary>
        public void ClearActions();

        /// <summary>
        /// The index of the current action.
        /// </summary>
        public IReadOnlyList<int> ActionIndices { get; }
    }
}
