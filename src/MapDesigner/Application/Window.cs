using System;
using System.Runtime.InteropServices;

namespace MapDesigner
{
    public class Window : INativeObject
    {
        public Window(string title, int width, int height, bool mainWindow = false)
        {
            if (!initialized)
            {
                InitWindow_();
                initialized = true;
            }
            mWindow = CreateWindow_(title, width, height, mainWindow);
            mWindowHandle = GetWindowHandle_(mWindow);
        }
        public void Loop()
        {
            while (!ShouldClose_(mWindow))
            {
                if (PeekMessage_(mWindow))
                {
                    RelayMessage_(mWindow);
                }
            }
        }
        ~Window()
        {
            DestroyWindow_(mWindow);
        }
        public IntPtr NativeInterface => mWindowHandle;
        private readonly IntPtr mWindow, mWindowHandle;
        [DllImport("MapDesigner-Internals.dll")]
        private static extern void InitWindow_();
        [DllImport("MapDesigner-Internals.dll")]
        private static extern IntPtr CreateWindow_(string title, int width, int height, bool mainWindow);
        [DllImport("MapDesigner-Internals.dll")]
        private static extern IntPtr GetWindowHandle_(IntPtr window);
        [DllImport("MapDesigner-Internals.dll")]
        private static extern bool PeekMessage_(IntPtr window);
        [DllImport("MapDesigner-Internals.dll")]
        private static extern void RelayMessage_(IntPtr window);
        [DllImport("MapDesigner-Internals.dll")]
        private static extern bool ShouldClose_(IntPtr window);
        [DllImport("MapDesigner-Internals.dll")]
        private static extern void DestroyWindow_(IntPtr window);
        private static bool initialized = false;
    }
}
