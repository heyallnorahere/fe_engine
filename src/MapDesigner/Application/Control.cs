using System;
using MapDesigner.Platform;

namespace MapDesigner
{
    public class Control : INativeObject
    {
        public Control(INativeObject parent, string className, string text, int width, int height, int x, int y)
        {
            uint style = 0x10000000 | 0x40000000;
            mControlHandle = PlatformWindowUtils.CreateWindowExA(0, className, text, style, x, y, width, height, parent.NativeInterface, new(0), new(0), new(0));
        }
        ~Control()
        {
            PlatformWindowUtils.DestroyWindow(mControlHandle);
        }
        public IntPtr NativeInterface => mControlHandle;
        private readonly IntPtr mControlHandle;
    }
}
