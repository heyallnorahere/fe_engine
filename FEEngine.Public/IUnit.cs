﻿/*
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
using System.Diagnostics.CodeAnalysis;

namespace FEEngine
{
    public struct UnitDesc : ICreationDesc
    {
        public UnitDesc()
        {
            Name = null;
            InitialInventory = null;
            EquippedWeapon = null;
            StartingPosition = (0, 0);
        }

        /// <summary>
        /// The name of this unit. Can be null.
        /// </summary>
        public string? Name;

        /// <summary>
        /// The items that this unit starts with. Can be null.
        /// </summary>
        public IList<IItem>? InitialInventory;

        /// <summary>
        /// The weapon that this unit starts with equipped.
        /// </summary>
        public IItem? EquippedWeapon;

        /// <summary>
        /// The position at which this unit starts.
        /// </summary>
        public Vector StartingPosition;

        public bool Verify() => true;
        public ICreationDesc Clone() => (ICreationDesc)MemberwiseClone();
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
        [MemberNotNull(nameof(Map))]
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

        /// <summary>
        /// Equips a weapon present in the inventory to <see cref="EquippedWeapon"/>.
        /// If a weapon is already equipped it will be unequipped.
        /// </summary>
        /// <param name="index">The index of the weapon. in the unit's inventory.</param>
        /// <returns>If the unit was able to equip the weapon.</returns>
        [MemberNotNullWhen(true, nameof(EquippedWeapon))]
        public bool EquipWeapon(int index);

        /// <summary>
        /// Unequips the item in <see cref="EquippedWeapon"/> to the inventory.
        /// </summary>
        /// <returns>
        /// The index of the item in the inventory,
        /// or -1 if <see cref="EquippedWeapon"/> is null.
        /// </returns>
        public int UnequipWeapon();

        /// <summary>
        /// Adds an item to <see cref="Inventory"/>.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>The index of the added item.</returns>
        public int AddItemToInventory(IItem item);

        /// <summary>
        /// Removes an item from <see cref="Inventory"/>.
        /// </summary>
        /// <param name="index">The index at which to remove an item.</param>
        /// <returns>The item removed, or null if that index does not exist.</returns>
        public IItem? RemoveItemFromInventory(int index);

        /// <summary>
        /// The weapon currently equipped.
        /// </summary>
        public IItem? EquippedWeapon { get; }

        /// <summary>
        /// A list of items that this unit has possesion of.
        /// </summary>
        public IReadOnlyList<IItem> Inventory { get; }
    }
}
