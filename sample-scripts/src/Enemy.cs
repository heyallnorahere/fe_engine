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
        public void OnUpdate()
        {
            List<Unit> units = new List<Unit>();
            for (ulong i = 0; i < Map.GetUnitCount(); i++)
            {
                Unit unit = Map.GetUnit(i);
                if (unit.Affiliation == Unit.UnitAffiliation.PLAYER)
                {
                    units.Add(unit);
                }
            }
            Vec2 delta = new Vec2(0, 0);
            foreach (Unit unit in units)
            {
                delta += unit.Position - this.Parent.Position;
            }
            delta /= units.Count;
            this.Parent.Move(delta);
            List<Unit> canAttack = new List<Unit>();
            if (this.Parent.HasWeaponEquipped())
            {
                Vec2 range = this.Parent.GetEquippedWeapon().Stats.Range;
                foreach (Unit unit in units)
                {
                    int distance = (unit.Position - this.Parent.Position).TaxicabLength();
                    if (distance >= range.X && distance <= range.Y)
                    {
                        canAttack.Add(unit);
                        Logger.Print(this.Parent.Index + ": Can attack unit " + unit.Index);
                    }
                }
            }
            Unit closest = this.Parent;
            foreach (Unit unit in canAttack)
            {
                if (unit.Index == this.Parent.Index)
                {
                    continue;
                }
                int distance = (unit.Position - this.Parent.Position).TaxicabLength();
                int closestDistance = (closest.Position - this.Parent.Position).TaxicabLength();
                if (distance < closestDistance || closest.Index == this.Parent.Index)
                {
                    closest = unit;
                }
            }
            if (closest.Index != this.Parent.Index)
            {
                this.Parent.Attack(closest);
                Logger.Print(this.Parent.Index + ": Attacked unit " + closest.Index, Renderer.Color.RED);
            }
        }
    }
}
