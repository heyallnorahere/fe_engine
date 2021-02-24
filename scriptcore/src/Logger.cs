using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace FEEngine
{
    public class Logger
    {
        public static void Print(String message, Renderer.Color color = Renderer.Color.WHITE)
        {
            Print_Native(message, color);
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Print_Native(String message, Renderer.Color color);
    }
}
