using System;
using System.Runtime.InteropServices;

namespace MapDesigner.Platform
{
    public class PlatformWindowUtils
    {
        [DllImport("user32.dll")]
        public static extern ushort RegisterClassA(IntPtr lpWndClass);
        [DllImport("user32.dll")]
        public static extern IntPtr LoadCursorA(IntPtr hInstance, IntPtr lpCursorName); // yes i know its a string, im tricking the compiler to allow me to pass 32512
        [DllImport("user32.dll")]
        public static extern IntPtr CreateWindowExA(uint dwExStyle, string lpClassName, string lpWindowName, uint dwStyle, int X, int Y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);
        [DllImport("user32.dll")]
        public static extern bool DestroyWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool PeekMessageA(IntPtr lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);
        [DllImport("user32.dll")]
        public static extern bool TranslateMessage(IntPtr lpMsg);
        [DllImport("user32.dll")]
        public static extern IntPtr DispatchMessageA(IntPtr lpMsg);
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibraryA(string lpLibFileName);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hLibModule);
        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();
    }
}
