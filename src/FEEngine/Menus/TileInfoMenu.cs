﻿using FEEngine.Math;

namespace FEEngine.Menus
{
    public class TileInfoMenu : IMenu
    {
        internal TileInfoMenu()
        {
            SelectedTile = new Vec2I();
        }
        public IVec2<int> MinSize { get => new Vec2I(20, 26 - Logger.MaxLogSize); }
        public IVec2<int> SelectedTile { get; internal set; }
        public string GetTitle()
        {
            return "Tile info";
        }
        public void Render(RenderContext context)
        {
            IVec2<int> origin = new Vec2I(0, mRenderSize.Y - 1);
            Registry registry = UIController.GameInstance.Registry;
            Register<Map> mapRegister = registry.GetRegister<Map>();
            Map map = mapRegister[UIController.GameInstance.CurrentMapIndex];
            Unit unit = map.GetUnitAt(SelectedTile);
            string unitDescText = "None";
            string affiliationName = "NA";
            Color affiliationColor = Color.White;
            if (unit != null)
            {
                unitDescText = string.Format("{0} ({1}/{2})", unit.Name, unit.CurrentHP, unit.Stats.HP);
                affiliationName = unit.Affiliation.ToString();
                if (unit.Affiliation == Unit.UnitAffiliation.ThirdEnemy)
                {
                    affiliationName = "Third army";
                }
                affiliationColor = Unit.GetColorForAffiliation(unit.Affiliation);
            }
            context.RenderString(MathUtil.AddVectors(origin, new Vec2I(1, -1)), string.Format("Unit: {0}", unitDescText));
            IVec2<int> affiliationTextPos = MathUtil.AddVectors(origin, new Vec2I(1, -3));
            string label = "Affiliation:";
            context.RenderString(affiliationTextPos, label);
            context.RenderString(MathUtil.AddVectors(affiliationTextPos, new Vec2I(2, -1)), affiliationName, affiliationColor);
        }
        public void SetSize(IVec2<int> size)
        {
            mRenderSize = size;
        }
        private IVec2<int> mRenderSize;
    }
}
