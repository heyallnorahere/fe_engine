using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEEngine
{
    class Test
    {
        private Unit unit;
        public void Init()
        {
            this.unit = Unit.GetUnitAt(new Math.Vec2(1, 1));
        }
        public void DoStuff()
        {
            Math.Vec2 pos = this.unit.Position;
            pos.Y++;
            this.unit.Position = pos;
        }
    }
}
