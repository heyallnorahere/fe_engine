using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using FEEngine.Math;

namespace FEEngine
{
    public class Weapon : Item
    {
        public struct WeaponStats
        {
            public uint Attack;
            public uint Hit;
            public uint Crit;
            public uint MaxDurability;
            public Vec2 Range;
        }
        public WeaponStats Stats
        {
            get
            {
                return GetStats_Native(this.parentIndex, this.Index);
            }
            set
            {
                SetStats_Native(this.parentIndex, this.Index, value);
            }
        }
        private Weapon(ulong parent, ulong inventoryIndex) : base(parent, inventoryIndex) { }
        public static new Weapon MakeFromInventoryIndex(Unit parent, ulong inventoryIndex)
        {
            return new Weapon(parent.Index, inventoryIndex);
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern WeaponStats GetStats_Native(ulong unit, ulong index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetStats_Native(ulong unit, ulong index, WeaponStats stats);
    }
}
