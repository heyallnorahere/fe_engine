using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace FEEngine
{
    class Map
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
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern ulong GetUnitCount_Native();
    }
}
