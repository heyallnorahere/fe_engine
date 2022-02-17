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

namespace FEEngine.Internal
{
    internal class DefaultUnit : IUnit
    {
        public DefaultUnit(UnitDesc desc)
        {
            Name = desc.Name;
            mPosition = desc.StartingPosition;
            mMap = null;
            mActionIndices = new List<int>();
            mEquippedWeapon = desc.EquippedWeapon;

            mInventory = new List<IItem>();
            if (desc.InitialInventory != null)
            {
                mInventory.AddRange(desc.InitialInventory);
            }
        }

        public string? Name { get; }
        public Vector Position
        {
            get => mPosition;
            set => mPosition = value;
        }

        public IMap? Map => mMap;
        public void SetMap(IMap map) => mMap = map;

        public bool AddAction(Action action)
        {
            if (mMap == null)
            {
                return false;
            }

            int index = mMap.PushAction(action);
            mActionIndices.Add(index);
            return true;
        }

        public void ClearActions() => mActionIndices.Clear();
        public IReadOnlyList<int> ActionIndices => mActionIndices;

        public bool EquipWeapon(int index)
        {
            if (index < 0 || index >= mInventory.Count)
            {
                return false;
            }

            IItem weapon = mInventory[index];
            if (weapon.WeaponData == null)
            {
                return false;
            }

            mInventory.RemoveAt(index);
            if (mEquippedWeapon != null)
            {
                mInventory.Add(mEquippedWeapon);
            }

            mEquippedWeapon = weapon;
            return true;
        }

        public int UnequipWeapon()
        {
            if (mEquippedWeapon == null)
            {
                return -1;
            }

            int index = mInventory.Count;
            mInventory.Add(mEquippedWeapon);
            mEquippedWeapon = null;
            return index;
        }

        public int AddItemToInventory(IItem item)
        {
            if (item.Owner != null)
            {
                return -1;
            }

            item.Owner = this;
            int index = mInventory.Count;
            mInventory.Add(item);
            return index;
        }

        public IItem? RemoveItemFromInventory(int index)
        {
            if (index < 0 || index > mInventory.Count)
            {
                return null;
            }

            IItem item = mInventory[index];
            mInventory.RemoveAt(index);
            return item;
        }

        public IItem? EquippedWeapon => mEquippedWeapon;
        public IReadOnlyList<IItem> Inventory => mInventory;

        private IItem? mEquippedWeapon;
        private readonly List<IItem> mInventory;
        private readonly List<int> mActionIndices;
        private Vector mPosition;
        private IMap? mMap;
    }
}
