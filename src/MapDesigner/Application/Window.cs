using System;
using System.Runtime.InteropServices;

namespace MapDesigner
{
    public class Window
    {
        public Window(string title, int width, int height, bool mainWindow = false)
        {
            if (!initialized)
            {
                InitWindow();
                initialized = true;
            }
            mWindow = CreateWindow(title, width, height, mainWindow);
        }
        public void Loop()
        {
            while (!ShouldClose(mWindow))
            {
                if (PeekMessage(mWindow))
                {
                    RelayMessage(mWindow);
                }
            }
        }
        ~Window()
        {
            DestroyWindow_(mWindow);
        }
        private readonly IntPtr mWindow;
        [DllImport("MapDesigner-Internals.dll")]
        private static extern void InitWindow();
        [DllImport("MapDesigner-Internals.dll")]
        private static extern IntPtr CreateWindow(string title, int width, int height, bool mainWindow);
        [DllImport("MapDesigner-Internals.dll")]
        private static extern bool PeekMessage(IntPtr window);
        [DllImport("MapDesigner-Internals.dll")]
        private static extern void RelayMessage(IntPtr window);
        [DllImport("MapDesigner-Internals.dll")]
        private static extern bool ShouldClose(IntPtr window);
        [DllImport("MapDesigner-Internals.dll")]
        private static extern void DestroyWindow_(IntPtr window);
        private static bool initialized = false;
    }
}
