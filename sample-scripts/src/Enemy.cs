using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FEEngine;
using FEEngine.Math;

namespace Scripts
{
    class Enemy : Behavior
    {
        private List<Unit> units;
        public void OnAttach()
        {
            this.units = new List<Unit>();
            for (ulong i = 0; i < Map.GetUnitCount(); i++)
            {
                Unit unit = Map.GetUnit(i);
                if (unit.Index == this.Parent.Index)
                {
                    continue;
                }
                this.units.Add(unit);
            }
        }
        public void OnUpdate()
        {
            Vec2 delta = new Vec2(0, 0);
            foreach (Unit u in units)
            {
                delta += u.Position - this.Parent.Position;
            }
            this.Parent.Move(delta);
        }
        public void OnRender(Renderer renderer)
        {
            Vec2 size = renderer.GetBufferSize();
            renderer.RenderStringAt(new Vec2(0, size.Y - 1), this.Parent.GetInventorySize().ToString(), Renderer.Color.RED);
        }
    }
}
