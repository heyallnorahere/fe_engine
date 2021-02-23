using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FEEngine.Math;

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
        public void OnUpdate()
        {
            Console.WriteLine("whoa wacky");
        }
    }
    class TestItem : ItemBehavior
    {
        public void OnUse()
        {
            Vec2 pos = this.Parent.Parent.Position;
            pos += new Vec2(1, 1);
            this.Parent.Parent.Position = pos;
        }
    }
}
