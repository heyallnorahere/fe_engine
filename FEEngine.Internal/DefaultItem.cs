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

using System;

namespace FEEngine.Internal
{
    internal class DefaultItem : IItem
    {
        public DefaultItem(ItemDesc desc)
        {
            Name = desc.Data.Name;
            Behavior = desc.Data.Behavior;
            WeaponData = desc.Data.WeaponData;
            MaxUses = desc.Data.MaxUses;
            
            int usesRemaining = desc.UsesRemaining;
            mUsesRemaining = usesRemaining <= 0 ? MaxUses : usesRemaining;
        }

        public string Name { get; }
        public ItemBehavior? Behavior { get; }
        public WeaponData? WeaponData { get; }
        public int MaxUses { get; }
        public int UsesRemaining => mUsesRemaining;
        public IUnit? Owner { get; set; }

        public bool OnItemUse(int uses)
        {
            if (uses < 1)
            {
                throw new ArgumentException("\"uses\" cannot be less than one!");
            }

            mUsesRemaining -= uses;
            return mUsesRemaining <= 0;
        }

        private int mUsesRemaining;
    }
}