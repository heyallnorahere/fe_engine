using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace FEEngine
{
    public class InputMapper
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
            return GetState_Native(this.memoryAddress);
        }
        private ulong memoryAddress;
        private InputMapper()
        {
            this.memoryAddress = 0;
        }
        public static InputMapper MakeFromMemoryAddress(ulong address)
        {
            InputMapper im = new InputMapper();
            im.memoryAddress = address;
            return im;
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Commands GetState_Native(ulong memoryAddress);
    }
}
