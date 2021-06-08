using System;
using System.Runtime.InteropServices;

namespace MapDesigner
{
    public class Control : INativeObject
    {
        public Control(INativeObject parent, string className, string text, int width, int height, int x, int y)
        {
            mControl = CreateControl_(parent.NativeInterface, className, text, width, height, x, y);
            mControlHandle = GetControlHandle_(mControl);
        }
        ~Control()
        {
            DestroyControl_(mControl);
        }
        public IntPtr NativeInterface => mControlHandle;
        private readonly IntPtr mControl, mControlHandle;
        [DllImport("MapDesigner-Internals.dll")]
        private static extern IntPtr CreateControl_(IntPtr parent, string className, string text, int width, int height, int x, int y);
        [DllImport("MapDesigner-Internals.dll")]
        private static extern IntPtr GetControlHandle_(IntPtr control);
        [DllImport("MapDesigner-Internals.dll")]
        private static extern void DestroyControl_(IntPtr control);
    }
}
