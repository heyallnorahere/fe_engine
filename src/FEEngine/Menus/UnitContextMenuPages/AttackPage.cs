using System.Collections.Generic;
using FEEngine.Math;

namespace FEEngine.Menus.UnitContextMenuPages
{
    internal class AttackPage : UnitContextMenu.Page
    {
        private class AttackUnitPage : UnitContextMenu.Page
        {
            public AttackUnitPage(Unit unit)
            {
                mUnit = unit;
            }
            protected override string GetTitle()
            {
                return string.Format("{0} ({1}/{2})", mUnit.Name, mUnit.CurrentHP, mUnit.Stats.HP);
            }
            protected override void OnSelect()
            {
                UIController.SelectedUnit.Attack(mUnit);
                Parent.GoBack();
                GoBack();
                UIController.ResetSelectedUnit();
                UIController.IsUnitContextMenuOpen = false;
            }
            private readonly Unit mUnit;
        }
        private static List<Unit> GetAttackableUnits()
        {
            List<Unit> units = new();
            Unit currentUnit = UIController.SelectedUnit;
            Item weapon = currentUnit.EquippedWeapon;
            Map map = currentUnit.Parent;
            if (weapon != null)
            {
                IVec2<int> range = weapon.WeaponStats.Range;
                foreach (Unit unit in map)
                {
                    int distance = MathUtil.SubVectors(currentUnit.Position, unit.Position).TaxicabLength();
                    if (distance < range.X || distance > range.Y || currentUnit.IsAllied(unit))
                    {
                        continue;
                    }
                    units.Add(unit);
                }
            }
            return units;
        }
        public static bool AreUnitsInRange()
        {
            return GetAttackableUnits().Count > 0;
        }
        protected override string GetTitle()
        {
            return "Attack";
        }
        protected override void OnSelect()
        {
            List<Unit> units = GetAttackableUnits();
            foreach (Unit unit in units)
            {
                AddChild(new AttackUnitPage(unit));
            }
        }
        internal override bool IsInternal { get { return true; } }
    }
}
