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

namespace FEEngine
{
    public delegate void ItemBehavior(IItem item, IUnit unit);
    public struct ItemData
    {
        /// <summary>
        /// The name of this item. Cannot be empty.
        /// </summary>
        public string Name = string.Empty;

        /// <summary>
        /// The delegate to be run when this item is used.
        /// Null behavior means this item cannot be used.
        /// </summary>
        public ItemBehavior? Behavior = null;

        /// <summary>
        /// The maximum number of times this item can be used.
        /// Must be greater than zero.
        /// </summary>
        public int MaxUses = 0;

        internal bool Verify() => Name.Length > 0 && MaxUses > 0;
    }

    public struct ItemDesc : ICreationDesc
    {
        /// <summary>
        /// The data that describes how this item acts.
        /// </summary>
        public ItemData Data = new ItemData();

        /// <summary>
        /// The amount of uses this item starts with.
        /// If this field is less than or equal to zero, uses <see cref="ItemData.MaxUses"/>.
        /// </summary>
        public int UsesRemaining = 0;

        public bool Verify() => Data.Verify();
    }

    /// <summary>
    /// An item is an object to be used by a <see cref="IUnit">unit</see>.
    /// </summary>
    [FactoryInterface]
    public interface IItem
    {
        /// <summary>
        /// The name of this item.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The function to call when this item is used.
        /// If this field is null, this item is not usable.
        /// </summary>
        public ItemBehavior? Behavior { get; }

        /// <summary>
        /// The maximum amount of times this item can be used.
        /// </summary>
        public int MaxUses { get; }

        /// <summary>
        /// This property represents many uses remaining until this item is rendered unusable.
        /// </summary>
        public int UsesRemaining { get; }

        /// <summary>
        /// This function is called when this item is used.
        /// This <b>DOES NOT</b> invoke the item's behavior.
        /// </summary>
        /// <param name="uses">
        ///     The amount of uses to subtract from <see cref="UsesRemaining"/>.
        ///     Cannot be less than 1.
        /// </param>
        /// <returns>If this item is to be removed from the unit's inventory.</returns>
        public bool OnItemUse(int uses = 1);
    }
}