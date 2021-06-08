using System;

namespace MapDesigner
{
    public interface INativeObject
    {
        IntPtr NativeInterface { get; }
    }
}
