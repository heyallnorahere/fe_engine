using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using FEEngine.Util;

namespace FEEngine
{
    public class InputMapper : RegisteredObject<InputMapper>
    {
        public struct Commands
        {
            public bool Up;
            public bool Down;
            public bool Left;
            public bool Right;
            public bool OK;
            public bool Back;
            public bool Exit;
        }
        public Commands GetState()
        {
            return GetState_Native(this.Index);
        }
        public ulong Index { get; private set; }
        public InputMapper()
        {
            this.Index = 0;
        }
        public void SetRegisterIndex(ulong index)
        {
            this.Index = index;
        }
        public static InputMapper MakeFromRegisterIndex(ulong index)
        {
            InputMapper im = new InputMapper();
            im.Index = index;
            return im;
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Commands GetState_Native(ulong index);
    }
}
