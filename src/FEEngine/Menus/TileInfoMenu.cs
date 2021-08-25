namespace FEEngine.Menus
{
    public class TileInfoMenu : IMenu
    {
        internal TileInfoMenu()
        {
            mRenderSize = new Vector2(0);
            SelectedTile = new Vector2();
        }
        public Vector2 MinSize => (20, 26 - Logger.MaxLogSize);
        public Vector2 SelectedTile { get; internal set; }
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
            var origin = new Vector2(0, mRenderSize.Y - 1);
            Registry registry = this.VerifyValue(UIController.GameInstance).Registry;
            Register<Map> mapRegister = registry.GetRegister<Map>();
            Map map = mapRegister[this.VerifyValue(UIController.GameInstance).CurrentMapIndex];
            Unit? unit = map.GetUnitAt(SelectedTile);
            string unitDescText = "None";
            string affiliationName = "NA";
            string className = "NA";
            Color affiliationColor = Color.White;
            if (unit != null)
            {
                unitDescText = string.Format("{0} ({1}/{2})", unit.Name, unit.CurrentHP, unit.BoostedStats.HP);
                affiliationName = unit.Affiliation.ToString();
                if (unit.Affiliation == Unit.UnitAffiliation.ThirdEnemy)
                {
                    affiliationName = "Third army";
                }
                affiliationColor = Unit.GetColorForAffiliation(unit.Affiliation);
                className = unit.Class.Name;
            }
            context.RenderString(origin + (1, -1), string.Format("Unit: {0}", unitDescText));
            Vector2 affiliationTextPos = origin + (1, -3);
            context.RenderString(affiliationTextPos, "Affiliation:");
            context.RenderString(affiliationTextPos + (2, -1), affiliationName, affiliationColor);
            Vector2 classTextPos = origin + (1, -6);
            context.RenderString(classTextPos, "Class:");
            context.RenderString(classTextPos + (2, -1), className);
            Tile? tile = map.GetTileAt(SelectedTile);
            string tileContentText = "Plain";
            string whoCanPassTile = GetStringForMovementLimit(Tile.MovementLimitEnum.All);
            if (tile != null)
            {
                // todo: figure out the tiles content
                whoCanPassTile = GetStringForMovementLimit(tile.MovementLimit);
            }
            Vector2 tileContentTextPos = origin + (1, -9);
            context.RenderString(tileContentTextPos, "Tile:");
            context.RenderString(tileContentTextPos + (2, -1), tileContentText);
            Vector2 whoCanPassTileTextPos = tileContentTextPos - (0, 2);
            context.RenderString(whoCanPassTileTextPos, "Supports:");
            context.RenderString(whoCanPassTileTextPos + (2, -1), whoCanPassTile);
        }
        public void SetSize(Vector2 size)
        {
            mRenderSize = size;
        }
        private Vector2 mRenderSize;
    }
}
