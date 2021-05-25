using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace FEEngine
{
    public class Game
    {
        public Game()
        {
            Console.WriteLine("just testing...");
        }
        public void Test()
        {
            Test_Native();
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void Test_Native();
    }
}
