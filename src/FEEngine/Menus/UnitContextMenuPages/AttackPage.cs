using System.Collections.Generic;

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
                UIController.SelectedUnit?.Attack(mUnit);
                Parent?.GoBack();
                GoBack();
                UIController.ResetSelectedUnit();
                UIController.IsUnitContextMenuOpen = false;
            }
            private readonly Unit mUnit;
        }
        private List<Unit> GetAttackableUnits()
        {
            List<Unit> units = new();
            Unit currentUnit = this.VerifyValue(UIController.SelectedUnit);
            Item? weapon = currentUnit.EquippedWeapon;
            Map map = Extensions.VerifyValue(null, currentUnit.Parent);
            if (weapon != null)
            {
                Vector2 range = Extensions.VerifyValue(null, weapon.WeaponStats).Range;
                foreach (Unit unit in map)
                {
                    int distance = (currentUnit.Position - unit.Position).TaxicabLength;
                    if (distance < range.X || distance > range.Y || currentUnit.IsAllied(unit))
                    {
                        continue;
                    }
                    units.Add(unit);
                }
            }
            return units;
        }
        public bool AreUnitsInRange()
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
    }
}
