using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using FEEngine.Util;

namespace FEEngine.UI
{
    public class UIController : RegisteredObject<UIController>
    {
        public ulong Index { get; private set; }
        public UIController()
        {
            this.Index = 0;
        }
        public void SetRegisterIndex(ulong index)
        {
            this.Index = index;
        }
        public Unit GetUnitMenuTarget()
        {
            if (!this.HasUnitSelected())
            {
                throw new Exception("No unit has been selected!");
            }
            return Unit.MakeFromIndex(GetUnitMenuTarget_Native(this.Index));
        }
        public bool HasUnitSelected()
        {
            return HasUnitSelected_Native(this.Index);
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern ulong GetUnitMenuTarget_Native(ulong index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool HasUnitSelected_Native(ulong index);
    }
}
