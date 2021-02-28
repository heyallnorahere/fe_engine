using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FEEngine;
using FEEngine.Math;

namespace Scripts
{
    public class UnitBehavior : Behavior
    {
        private bool IsAllied(Unit unit)
        {
            bool allied = unit.Affiliation == this.Parent.Affiliation;
            if (this.Parent.Affiliation == Unit.UnitAffiliation.ALLY)
            {
                allied = allied || (unit.Affiliation == Unit.UnitAffiliation.PLAYER);
            }
            if (this.Parent.Affiliation == Unit.UnitAffiliation.PLAYER)
            {
                allied = allied || (unit.Affiliation == Unit.UnitAffiliation.ALLY);
            }
            return allied;
        }
        private List<Unit> GetUnitsInRange()
        {
            List<Unit> units = new List<Unit>();
            for (ulong i = 0; i < Map.GetUnitCount(); i++)
            {
                Unit unit = Map.GetUnit(i);
                if (!this.IsAllied(unit))
                {
                    units.Add(unit);
                }
            }
            List<Unit> inRange = new List<Unit>();
            foreach (Unit unit in units)
            {
                Vec2<int> difference = unit.Position - this.Parent.Position;
                int distance = difference.TaxicabLength();
                int maxRange = 0;
                if (this.Parent.HasWeaponEquipped())
                {
                    maxRange = this.Parent.GetEquippedWeapon().Stats.Range.Y;
                }
                if (distance <= this.Parent.CurrentMovement + maxRange)
                {
                    inRange.Add(unit);
                }
            }
            return inRange;
        }
        private Unit DetermineWeakestUnit(List<Unit> units)
        {
            bool magic = false;
            if (this.Parent.HasWeaponEquipped())
            {
                Weapon weapon = this.Parent.GetEquippedWeapon();
                Weapon.WeaponType type = weapon.Type;
                magic = (type == Weapon.WeaponType.BLACKMAGIC || type == Weapon.WeaponType.DARKMAGIC);
            }
            Unit unit = this.Parent;
            foreach (Unit u in units)
            {
                Unit.UnitStats stats = u.Stats;
                Unit.UnitStats prevStats = unit.Stats;
                uint defense = magic ? stats.Resistance : stats.Defense;
                uint prevDefense = magic ? prevStats.Resistance : prevStats.Defense;
                if (defense < prevDefense || unit.Index == this.Parent.Index)
                {
                    unit = u;
                }
            }
            if (unit.Index == this.Parent.Index)
            {
                throw new Exception("Unit list is empty!");
            }
            return unit;
        }
        public void OnUpdate(InputMapper inputMapper)
        {
            List<Unit> inRange = this.GetUnitsInRange();
            if (inRange.Count == 0)
            {
                this.Parent.Wait();
                return;
            }
            Unit weakestUnit = this.DetermineWeakestUnit(inRange);
            Vec2<int> offset = weakestUnit.Position - this.Parent.Position;

            this.Parent.Wait();
        }
    }
}
