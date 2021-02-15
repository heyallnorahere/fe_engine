using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEEngine
{
    class Test
    {
        int x = 0;
        public void DoStuff()
        {
            Unit u = Unit.GetUnitAt(new Math.Vec2(1 + x, 1));
            Math.Vec2 pos = u.Position;
            pos.X++;
            this.x++;
            u.Position = pos;
        }
    }
}
