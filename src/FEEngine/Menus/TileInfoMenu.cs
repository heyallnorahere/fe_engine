using FEEngine.Math;

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
        private string GetStringForMovementLimit(Tile.MovementLimitEnum movementLimit)
        {
            return movementLimit switch
            {
                Tile.MovementLimitEnum.Flying => "Flying units",
                Tile.MovementLimitEnum.All => "All units",
                _ => "No units",
            };
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
            string className = "NA";
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
                className = unit.Class.Name;
            }
            context.RenderString(MathUtil.AddVectors(origin, new Vec2I(1, -1)), string.Format("Unit: {0}", unitDescText));
            IVec2<int> affiliationTextPos = MathUtil.AddVectors(origin, new Vec2I(1, -3));
            context.RenderString(affiliationTextPos, "Affiliation:");
            context.RenderString(MathUtil.AddVectors(affiliationTextPos, new Vec2I(2, -1)), affiliationName, affiliationColor);
            IVec2<int> classTextPos = MathUtil.AddVectors(origin, new Vec2I(1, -6));
            context.RenderString(classTextPos, "Class:");
            context.RenderString(MathUtil.AddVectors(classTextPos, new Vec2I(2, -1)), className);
            Tile tile = map.GetTileAt(SelectedTile);
            string tileContentText = "Plain";
            string whoCanPassTile = GetStringForMovementLimit(Tile.MovementLimitEnum.All);
            if (tile != null)
            {
                // todo: figure out the tiles content
                whoCanPassTile = GetStringForMovementLimit(tile.MovementLimit);
            }
            IVec2<int> tileContentTextPos = MathUtil.AddVectors(origin, new Vec2I(1, -9));
            context.RenderString(tileContentTextPos, "Tile:");
            context.RenderString(MathUtil.AddVectors(tileContentTextPos, new Vec2I(2, -1)), tileContentText);
            IVec2<int> whoCanPassTileTextPos = MathUtil.SubVectors(tileContentTextPos, new Vec2I(0, 2));
            context.RenderString(whoCanPassTileTextPos, "Supports:");
            context.RenderString(MathUtil.AddVectors(whoCanPassTileTextPos, new Vec2I(2, -1)), whoCanPassTile);
        }
        public void SetSize(IVec2<int> size)
        {
            mRenderSize = size;
        }
        private IVec2<int> mRenderSize;
    }
}
