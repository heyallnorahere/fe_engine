using System.Collections.Generic;
using FEEngine.Math;

namespace FEEngine.Menus.UnitContextMenuPages
{
    internal class GambitPage : UnitContextMenu.Page
    {
        private class UnitSelectionPage : UnitContextMenu.Page
        {
            public UnitSelectionPage(Unit unit)
            {
                mUnit = unit;
            }
            protected override void OnSelect()
            {
                Unit selectedUnit = this.VerifyValue(UIController.SelectedUnit);
                Battalion battalion = this.VerifyValue(selectedUnit.Battalion);
                Gambit gambit = battalion.GetGambit();
                gambit.Use(selectedUnit, mUnit, battalion);
                Parent?.GoBack();
                GoBack();
                UIController.ResetSelectedUnit();
                UIController.IsUnitContextMenuOpen = false;
            }
            protected override string GetTitle() => $"{mUnit.Name} ({mUnit.CurrentHP}/{mUnit.Stats.HP})";
            private readonly Unit mUnit;
        }
        private List<Unit> GetUnitsInRange(Gambit gambit)
        {
            List<Unit> units = new();
            Unit selectedUnit = this.VerifyValue(UIController.SelectedUnit);
            Map map = this.VerifyValue(selectedUnit.Parent);
            IVec2<int> range = gambit.Range;
            foreach (Unit unit in map)
            {
                int distance = MathUtil.SubVectors(selectedUnit.Position, unit.Position).TaxicabLength();
                if (distance < range.X || distance > range.Y)
                {
                    continue;
                }
                if (selectedUnit.IsAllied(unit))
                {
                    continue;
                }
                units.Add(unit);
            }
            return units;
        }
        public bool IsAvailable()
        {
            Unit unit = this.VerifyValue(UIController.SelectedUnit);
            Battalion? battalion = unit.Battalion;
            if (battalion == null)
            {
                return false;
            }
            else
            {
                if (!battalion.HasGambit())
                {
                    return false;
                }
                Gambit gambit = battalion.GetGambit();
                if (gambit.GambitType != GambitType.PhysicalAttack && gambit.GambitType != GambitType.MagicAttack)
                {
                    return false;
                }
                return GetUnitsInRange(gambit).Count > 0;
            }
        }
        protected override string GetTitle() => "Gambit";
        protected override void UpdatePage()
        {
            Gambit gambit = this.VerifyValue(UIController.SelectedUnit?.Battalion?.GetGambit());
            List<Unit> unitsInRange = GetUnitsInRange(gambit);
            foreach (Unit unit in unitsInRange)
            {
                AddChild(new UnitSelectionPage(unit));
            }
        }
    }
}
