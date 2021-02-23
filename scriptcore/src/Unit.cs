using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using FEEngine.Math;
namespace FEEngine {
    public class Unit {
        public enum Affiliation
        {
            player,
            enemy,
            third_army,
            ally,
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
        public Vec2 Position
        {
            get
            {
                Vec2 pos;
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
        public void Move(Vec2 offset)
        {
            Vec2 to_move = offset;
            int taxicab_length = to_move.TaxicabLength();
            if (taxicab_length > this.CurrentMovement)
            {
                float x = (float)to_move.X / (float)taxicab_length;
                float y = (float)to_move.Y / (float)taxicab_length;
                to_move = new Vec2((int)(x * this.CurrentMovement), (int)(y * this.CurrentMovement));
            }
            Move_Native(this.Index, to_move);
        }
        public ulong GetInventorySize()
        {
            return GetInventorySize_Native(this.Index);
        }
        public Item GetInventoryItem(ulong index)
        {
            return Item.MakeFromInventoryIndex(this, index);
        }
        internal Unit(ulong index)
        {
            this.Index = index;
        }
        protected Unit()
        {
            this.Index = 0;
        }
        public static Unit GetUnitAt(Vec2 position)
        {
            return new Unit(GetUnitAt_Native(position));
        }
        public static Unit MakeFromIndex(ulong index)
        {
            return new Unit(index);
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void GetPosition_Native(ulong index, out Vec2 position);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetPosition_Native(ulong index, Vec2 position);
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
        private static extern void Move_Native(ulong index, Vec2 offset);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern ulong GetUnitAt_Native(Vec2 position);
    }
}
