using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FEEngine
{
    class Test
    {
        private Unit unit;
        public void Init()
        {
            this.unit = Map.GetUnit(0);
        }
        public void DoStuff()
        {
            Math.Vec2 pos = this.unit.Position;
            pos.Y++;
            this.unit.Position = pos;
        }
    }
    class TestBehavior : Behavior
    {
        public void OnAttach()
        {
            // todo: attach
        }
        public void OnDetach()
        {
            // todo: detach
        }
        public void OnUpdate()
        {
            Console.WriteLine("whoa wacky");
        }
    }
}
