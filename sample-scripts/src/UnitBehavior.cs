using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FEEngine;
using FEEngine.Math;
using FEEngine.Util;

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
            // player units should never have a behavior, but just in case
            if (this.Parent.Affiliation == Unit.UnitAffiliation.PLAYER)
            {
                allied = allied || (unit.Affiliation == Unit.UnitAffiliation.ALLY);
            }
            return allied;
        }
        private List<Unit> GetUnitsInRange()
        {
            List<Unit> units = new List<Unit>();
            // unit cant attack
            if (!this.Parent.HasWeaponEquipped())
            {
                return units;
            }
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
                int maxRange = this.Parent.GetEquippedWeapon().Stats.Range.Y;
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
        private Vec2<int> CalcSurroundingTile(Vec2<int> target, bool isCandidate = false, bool farthest = true) {
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
                        if (Map.IsTileOccupied(candidate) && !isCandidate) { 
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
                // First, take the position furthest from/closest to the target;
                var tDistA = (a - target).TaxicabLength();
                var tDistB = (b - target).TaxicabLength();
                if (tDistA < tDistB)
                {
                    return farthest ? 1 : -1;
                }
                else if (tDistA > tDistB)
                {
                    return farthest ? -1 : 1;
                }

                // If those are equal, take the position closest to/farthest from the parent.
                var myDistA = (a - this.Parent.Position).TaxicabLength();
                var myDistB = (b - this.Parent.Position).TaxicabLength();
                if (myDistA < myDistB)
                {
                    return farthest ? -1 : 1;
                }
                else if (myDistA > myDistB)
                {
                    return farthest ? 1 : -1;
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
                Vec2<int> delta = new Vec2<int>();
                for (ulong i = 0; i < Map.GetUnitCount(); i++)
                {
                    Unit unit = Map.GetUnit(i);
                    if (!this.IsAllied(unit))
                    {
                        delta += unit.Position - this.Parent.Position;
                    }
                }
                if (delta.TaxicabLength() > this.Parent.CurrentMovement)
                {
                    Vec2<double> normal = delta.Normalize();
                    normal *= this.Parent.CurrentMovement * 0.75;
                    delta.X = (int)Math.Floor(normal.X);
                    delta.Y = (int)Math.Floor(normal.Y);
                }
                if (Map.IsTileOccupied(this.Parent.Position))
                {
                    Vec2<int> destination = this.Parent.Position + delta;
                    Vec2<int> pos = this.CalcSurroundingTile(destination, true, false);
                    delta = pos - this.Parent.Position;
                }
                this.Parent.Move(delta);
                return;
            }
            Unit weakestUnit = this.DetermineWeakestUnit(inRange);
            Vec2<int> targetTile = this.CalcSurroundingTile(weakestUnit.Position);
            Vec2<int> difference = targetTile - this.Parent.Position;
            this.Parent.Move(difference);
            this.Parent.Attack(weakestUnit);
        }
    }
}
