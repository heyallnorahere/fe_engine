using System;
using System.Runtime.InteropServices;
using MapDesigner.Platform;

namespace MapDesigner
{
    public class Window : INativeObject
    {
        public Window(string title, int width, int height, bool mainWindow = false)
        {
            if (!initialized)
            {
                if (!InitWindow())
                {
                    Console.WriteLine("Error: {0}", PlatformWindowUtils.GetLastError());
                    throw new Exception();
                }
                initialized = true;
            }
            uint style = 0x10000000 | 0x00000000 | 0x00C00000 | 0x00080000 | 0x00040000 | 0x00020000 | 0x00010000;
            mWindowHandle = PlatformWindowUtils.CreateWindowExA(0, "MapDesigner", title, style, -1, -1, width, height, new(0), new(0), new(0), new(0));
            if (mWindowHandle.ToInt32() == 0)
            {
                Console.WriteLine("Error: {0}", PlatformWindowUtils.GetLastError());
                throw new NullReferenceException();
            }
        }
        public unsafe void Loop()
        {
            MSG msg = new();
            while (msg.message != 0x12)
            {
                if (PlatformWindowUtils.PeekMessageA(new(&msg), new(0), 0, 0, 1)) // 1 being PM_REMOVE
                {
                    PlatformWindowUtils.TranslateMessage(new(&msg));
                    PlatformWindowUtils.DispatchMessageA(new(&msg));
                }
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct WNDCLASSA
        {
            public uint style;
            public IntPtr lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public IntPtr lpszMenuName;
            public IntPtr lpszClassName;
            public void Zero()
            {
                style = 0;
                lpfnWndProc = new(0);
                cbClsExtra = 0;
                cbWndExtra = 0;
                hInstance = new(0);
                hIcon = new(0);
                hCursor = new(0);
                hbrBackground = new(0);
                lpszMenuName = new(0);
                lpszClassName = new(0);
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x, y;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct MSG
        {
            public IntPtr hwnd;
            public uint message;
            public UIntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public POINT pt;
            public uint lPrivate;
        }
        private unsafe static bool InitWindow()
        {
            string className = "MapDesigner";
            WNDCLASSA cls = new();
            cls.Zero();
            int length = sizeof(char) * (className.Length + 1);
            cls.lpszClassName = Marshal.AllocHGlobal(length);
            char[] buf = new char[className.Length + 1];
            buf[className.Length] = '\0';
            for (int i = 0; i < className.Length; i++)
            {
                buf[i] = className[i];
            }
            cls.lpszClassName = Marshal.AllocHGlobal(buf.Length);
            for (int i = 0; i < buf.Length; i++)
            {
                char[] temp = new char[1];
                temp[0] = buf[i];
                Marshal.Copy(temp, 0, cls.lpszClassName + i, 1);
            }
            cls.hbrBackground = new(5); // COLOR_WINDOW
            cls.hCursor = PlatformWindowUtils.LoadCursorA(new(0), new(32512)); // IDC_ARROW
            IntPtr module = PlatformWindowUtils.LoadLibraryA("MapDesigner-Internals.dll");
            cls.lpfnWndProc = PlatformWindowUtils.GetProcAddress(module, "wndproc");
            if (PlatformWindowUtils.RegisterClassA(new(&cls)) == 0)
            {
                return false;
            }
            Marshal.FreeHGlobal(cls.lpszClassName);
            return true;
        }
        ~Window()
        {
            PlatformWindowUtils.DestroyWindow(mWindowHandle);
        }
        public IntPtr NativeInterface => mWindowHandle;
        private readonly IntPtr mWindowHandle;
        private static bool initialized = false;
    }
}
