using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using FEEngine.Math;

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
            return Unit.MakeFromIndex(index);
        }
        public static Vec2 GetSize()
        {
            return GetSize_Native();
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern ulong GetUnitCount_Native();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Vec2 GetSize_Native();
    }
}
