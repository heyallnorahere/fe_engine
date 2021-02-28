using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using FEEngine.Math;

namespace FEEngine {
    public class Unit {
        public enum UnitAffiliation
        {
            PLAYER,
            ENEMY,
            THIRD_ARMY,
            ALLY,
        }
        public struct UnitStats
        {
            public uint Level;
            public uint MaxHP;
            public uint Strength;
            public uint Magic;
            public uint Dexterity;
            public uint Speed;
            public uint Luck;
            public uint Defense;
            public uint Resistance;
            public uint Charm;
            public uint Movement;
        }
        public ulong Index { get; private set; }
        public String Name
        {
            get
            {
                return GetName_Native(this.Index);
            }
            set
            {
                SetName_Native(this.Index, value);
            }
        }
        public Vec2<int> Position
        {
            get
            {
                Vec2<int> pos;
                GetPosition_Native(this.Index, out pos);
                return pos;
            }
            set
            {
                SetPosition_Native(this.Index, value);
            }
        }
        public uint HP
        {
            get
            {
                return GetHP_Native(this.Index);
            }
            set
            {
                SetHP_Native(this.Index, value);
            }
        }
        public uint CurrentMovement
        {
            get
            {
                return GetCurrentMovement_Native(this.Index);
            }
            private set
            {
                SetCurrentMovement_Native(this.Index, value);
            }
        }
        public UnitStats Stats
        {
            get
            {
                return GetStats_Native(this.Index);
            }
            set
            {
                SetStats_Native(this.Index, value);
            }
        }
        public UnitAffiliation Affiliation
        {
            get
            {
                return GetAffiliation_Native(this.Index);
            }
        }
        public void Move(Vec2<int> offset)
        {
            Vec2<int> to_move = offset;
            int taxicab_length = to_move.TaxicabLength();
            if (taxicab_length > this.CurrentMovement)
            {
                float x = (float)to_move.X / (float)taxicab_length;
                float y = (float)to_move.Y / (float)taxicab_length;
                to_move = new Vec2<int>((int)(x * this.CurrentMovement), (int)(y * this.CurrentMovement));
            }
            Move_Native(this.Index, to_move);
        }
        public void Attack(Unit other)
        {
            if (this.HP <= 0 || other.HP <= 0)
            {
                throw new Exception("One of the units specified does not exist!");
            }
            Attack_Native(this.Index, other.Index);
        }
        public void Wait()
        {
            Wait_Native(this.Index);
        }
        public ulong GetInventorySize()
        {
            return GetInventorySize_Native(this.Index);
        }
        public Item GetInventoryItem(ulong index)
        {
            return Item.MakeFromInventoryIndex(this, index);
        }
        public Weapon GetInventoryWeapon(ulong index)
        {
            return Weapon.MakeFromInventoryIndex(this, index);
        }
        public Weapon GetEquippedWeapon()
        {
            if (!this.HasWeaponEquipped())
            {
                throw new Exception("No weapon has been equipped!");
            }
            return Weapon.MakeFromInventoryIndex(this, this.GetInventorySize());
        }
        public bool HasWeaponEquipped()
        {
            return HasWeaponEquipped_Native(this.Index);
        }
        // after calling, DO NOT keep the passed object. it will refer to a different item.
        public Weapon EquipWeapon(Weapon weapon)
        {
            if (weapon.Parent.Index != this.Index)
            {
                throw new Exception("The passed weapon does not exist in this unit's inventory!");
            }
            if (!weapon.IsWeapon())
            {
                throw new Exception("The passed weapon is not a weapon!");
            }
            Equip_Native(this.Index, weapon.Index);
            return this.GetEquippedWeapon();
        }
        internal Unit(ulong index)
        {
            this.Index = index;
        }
        protected Unit()
        {
            this.Index = 0;
        }
        public static Unit MakeFromIndex(ulong index)
        {
            return new Unit(index);
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern String GetName_Native(ulong index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetName_Native(ulong index, String name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void GetPosition_Native(ulong index, out Vec2<int> position);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetPosition_Native(ulong index, Vec2<int> position);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern uint GetHP_Native(ulong index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetHP_Native(ulong index, uint hp);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern uint GetCurrentMovement_Native(ulong index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetCurrentMovement_Native(ulong index, uint movement);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern UnitStats GetStats_Native(ulong index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetStats_Native(ulong index, UnitStats stats);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern ulong GetInventorySize_Native(ulong index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern UnitAffiliation GetAffiliation_Native(ulong index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Move_Native(ulong index, Vec2<int> offset);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Attack_Native(ulong index, ulong otherIndex);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Wait_Native(ulong index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Equip_Native(ulong index, ulong itemIndex);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool HasWeaponEquipped_Native(ulong index);
    }
}
