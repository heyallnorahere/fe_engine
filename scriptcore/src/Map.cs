using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using FEEngine.Math;
using FEEngine.Util;

namespace FEEngine
{
    public class Map
    {
        public static ulong GetUnitCount()
        {
            return GetUnitCount_Native();
        }
        public static Unit GetUnit(ulong index)
        {
            if (index >= GetUnitCount())
            {
                throw new Exception("Index exceeded unit count!");
            }
            return Unit.MakeFromIndex(GetUnit_Native(index));
        }
        public static Unit GetUnitAt(Vec2<int> position)
        {
            if (!IsTileOccupied(position))
            {
                throw new Exception("Specified tile was not occupied!");
            }
            return Unit.MakeFromIndex(GetUnitAt_Native(position));
        }
        public static Vec2<int> GetSize()
        {
            return GetSize_Native();
        }
        public static bool IsTileOccupied(Vec2<int> position)
        {
            return IsTileOccupied_Native(position);
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern ulong GetUnitCount_Native();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Vec2<int> GetSize_Native();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern ulong GetUnit_Native(ulong index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern ulong GetUnitAt_Native(Vec2<int> position);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool IsTileOccupied_Native(Vec2<int> position);
    }
}
