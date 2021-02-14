using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using FEEngine.Math;
namespace FEEngine {
    public class Unit {
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
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void GetPosition_Native(ulong index, out Vec2 position);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetPosition_Native(ulong index, Vec2 position);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern uint GetHP_Native(ulong index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetHP_Native(ulong index, uint hp);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern ulong GetUnitAt_Native(Vec2 position);
    }
}
