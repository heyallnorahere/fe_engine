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

    public enum WeaponType
    {
        None,
        Sword,
        Axe,
        Lance,
        Bow,
        Gauntlets,
        Magic
    }

    public struct WeaponData
    {
        public WeaponData()
        {
            Might = Weight = Hit = Crit = MinRange = MaxRange = -1;
            Type = WeaponType.None;
        }

        /// <summary>
        /// How much damage this weapon deals on its own.
        /// Must be greater than 0.
        /// </summary>
        public int Might;

        /// <summary>
        /// How much this weapon slows down the unit.
        /// Must be greater than 0.
        /// </summary>
        public int Weight;

        /// <summary>
        /// The base hit chance.
        /// Must be greater than 0 and less than or equal to 100.
        /// </summary>
        public int Hit;

        /// <summary>
        /// The chance of triggering a critical hit.
        /// Must be greater than or equal to 0 and less than or equal to 100.
        /// </summary>
        public int Crit;

        /// <summary>
        /// The minimum amount of tiles between the user and the target for this weapon to be used.
        /// Must be greater than 0, and less than or equal to <see cref="MaxRange"/>.
        /// </summary>
        public int MinRange;

        /// <summary>
        /// The maximum amount of tiles between the user and the target for this weapon to be used.
        /// Must be greater than 0, and greater than or equal to <see cref="MinRange"/>.
        /// </summary>
        public int MaxRange;

        /// <summary>
        /// The type of this weapon. Cannot be <see cref="WeaponType.None"/>.
        /// </summary>
        public WeaponType Type;

        internal bool Verify()
        {
            return Might > 0 &&
                Weight > 0 &&
                Hit > 0 && Hit <= 100 &&
                Crit >= 0 && Crit <= 100 &&
                MinRange > 0 &&
                MaxRange > 0 &&
                MaxRange >= MinRange &&
                Type != WeaponType.None;
        }
    }

    public struct ItemData
    {
        public ItemData()
        {
            Name = string.Empty;
            Behavior = null;
            WeaponData = null;
            MaxUses = 0;
        }

        /// <summary>
        /// The name of this item. Cannot be empty.
        /// </summary>
        public string Name;

        /// <summary>
        /// The delegate to be run when this item is used.
        /// Null behavior means this item cannot be used.
        /// However, if this is not null, <see cref="WeaponData"/> must be null.
        /// </summary>
        public ItemBehavior? Behavior;

        /// <summary>
        /// This field describes how this item acts as a weapon.
        /// Null weapon data means this item is not a weapon.
        /// However, if this is not null, <see cref="Behavior"/> must be null.
        /// </summary>
        public WeaponData? WeaponData;

        /// <summary>
        /// The maximum number of times this item can be used.
        /// Must be greater than zero.
        /// </summary>
        public int MaxUses;

        internal bool Verify()
        {
            bool itemTypeDetermined = false;
            foreach (var item in new object?[]
            {
                Behavior,
                WeaponData
            })
            {
                if (item != null)
                {
                    if (itemTypeDetermined)
                    {
                        return false;
                    }

                    if (item is WeaponData weaponData && !weaponData.Verify())
                    {
                        return false;
                    }

                    itemTypeDetermined = true;
                }
            }

            return Name.Length > 0 && MaxUses > 0 && (WeaponData?.Verify() ?? true);
        }
    }

    public struct ItemDesc : ICreationDesc
    {
        public ItemDesc()
        {
            Data = new ItemData();
            UsesRemaining = 0;
        }

        /// <summary>
        /// The data that describes how this item acts.
        /// </summary>
        public ItemData Data;

        /// <summary>
        /// The amount of uses this item starts with.
        /// If this field is less than or equal to zero, uses <see cref="ItemData.MaxUses"/>.
        /// </summary>
        public int UsesRemaining;

        public bool Verify() => Data.Verify() && UsesRemaining <= Data.MaxUses;
        public ICreationDesc Clone() => (ICreationDesc)MemberwiseClone();
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
        /// The data that describes how this weapon acts.
        /// If this property is null, this item is not a weapon.
        /// </summary>
        public WeaponData? WeaponData { get; }

        /// <summary>
        /// The maximum amount of times this item can be used.
        /// </summary>
        public int MaxUses { get; }

        /// <summary>
        /// This property represents many uses remaining until this item is rendered unusable.
        /// </summary>
        public int UsesRemaining { get; }

        /// <summary>
        /// The unit that owns this item.
        /// </summary>
        public IUnit? Owner { get; set; }

        /// <summary>
        /// This function is called when this item is used.
        /// This <b>DOES NOT</b> invoke the item's behavior.
        /// </summary>
        /// <param name="uses">
        /// The amount of uses to subtract from <see cref="UsesRemaining"/>.
        /// Cannot be less than 1.
        /// </param>
        /// <returns>If this item is to be removed from the unit's inventory.</returns>
        public bool OnItemUse(int uses = 1);
    }
}