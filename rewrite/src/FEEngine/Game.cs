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
            mRegistry = new Registry();
        }
        public Registry Registry
        {
            get
            {
                return mRegistry;
            }
        }
        private Registry mRegistry;
    }
}
