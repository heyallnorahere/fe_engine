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
        private Vec2<int> CalcSurroundingTile(Vec2<int> target) {
            Vec2<int> size = Map.GetSize();

            // Calculate all of the candidate positions
            var candidates = new List<Vec2<int>>();
            var weaponRange = this.Parent.GetEquippedWeapon().Stats.Range;

            var maxDist = weaponRange.Y;
            for (var dx = -maxDist; dx <= maxDist; dx++) {
                for (var dy = -maxDist; dy <= maxDist; dy++) {
                    var myDist = new Vec2<int>(dx, dy).TaxicabLength();
                    if (myDist >= weaponRange.X && myDist <= weaponRange.Y) {
                        // This is a good spot for us.
                        var candidate = new Vec2<int>(dx, dy) + target;
                        if ((candidate - this.Parent.Position).TaxicabLength() > this.Parent.CurrentMovement) {
                            continue;
                        }
                        if (Map.IsTileOccupied(candidate)) { 
                            continue; 
                        }
                        if (candidate.X < 0 || candidate.Y < 0 || candidate.X >= size.X || candidate.Y >= size.Y) {
                            continue;
                        }
                        candidates.Add(candidate);
                    }
                }
            }

            if (candidates.Count == 0) {
                return this.Parent.Position;
            }

            // Out of the positions we have, which is best? 
            candidates.Sort((a, b) => {
                // First, take the position furthest from the target;
                var tDistA = (a - target).TaxicabLength();
                var tDistB = (b - target).TaxicabLength();
                if (tDistA < tDistB) {
                    return 1;
                } else if (tDistA > tDistB) {
                    return -1;
                }

                // If those are equal, take the position closest to the parent.
                var myDistA = (a - this.Parent.Position).TaxicabLength();
                var myDistB = (b - this.Parent.Position).TaxicabLength();
                if (myDistA < myDistB) {
                    return -1;
                } else if (myDistA > myDistB) {
                    return 1;
                }

                // shrug
                return 0;
            });


            return candidates[0];
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
            Vec2<int> targetTile = this.CalcSurroundingTile(weakestUnit.Position);
            Vec2<int> difference = targetTile - this.Parent.Position;
            this.Parent.Move(difference);
            this.Parent.Attack(weakestUnit);
            this.Parent.Wait();
        }
    }
}
