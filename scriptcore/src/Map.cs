using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using FEEngine.Math;
using FEEngine.Util;

namespace FEEngine
{
    public class Map : RegisteredObject<Map>
    {
        public ulong Index { get; private set; }
        public ulong GetUnitCount()
        {
            return GetUnitCount_Native(this.Index);
        }
        public Unit GetUnit(ulong index)
        {
            if (index >= this.GetUnitCount())
            {
                throw new Exception("Index exceeded unit count!");
            }
            return Unit.MakeFromIndex(GetUnit_Native(this.Index, index));
        }
        public Unit GetUnitAt(Vec2<int> position)
        {
            if (!IsTileOccupied(position))
            {
                throw new Exception("Specified tile was not occupied!");
            }
            return Unit.MakeFromIndex(GetUnitAt_Native(this.Index, position));
        }
        public Vec2<int> GetSize()
        {
            return GetSize_Native(this.Index);
        }
        public bool IsTileOccupied(Vec2<int> position)
        {
            return IsTileOccupied_Native(this.Index, position);
        }
        public Map()
        {
            this.Index = 0;
        }
        public void SetRegisterIndex(ulong index)
        {
            this.Index = index;
        }
        public static Map MakeFromRegisterIndex(ulong index)
        {
            Map map = new Map();
            map.Index = index;
            return map;
        }
        public static Map GetMap()
        {
            return ObjectRegistry.GetRegister<Map>().Get(0);
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern ulong GetUnitCount_Native(ulong index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Vec2<int> GetSize_Native(ulong index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern ulong GetUnit_Native(ulong index, ulong unitIndex);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern ulong GetUnitAt_Native(ulong index, Vec2<int> position);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool IsTileOccupied_Native(ulong index, Vec2<int> position);
    }
}
