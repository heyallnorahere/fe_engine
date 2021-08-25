namespace FEEngine
{
    /// <summary>
    /// A <see cref="BorderedMenu"/> is a <see cref="BorderedObject"/> with a title bar
    /// </summary>
    public class BorderedMenu : BorderedObject
    {
        public BorderedMenu(IMenu menu) : base(menu)
        {
            mMenu = menu;
        }
        protected override void DrawHorizontalLine(int y, RenderContext context)
        {
            if (y == RenderSize.Y - 1)
            {
                string title = mMenu.GetTitle();
                int lineWidth = RenderSize.X - 2;
                int titleLength = title.Length;
                if (titleLength % 2 > 0)
                {
                    titleLength++;
                }
                int lineSegmentWidth = (lineWidth - titleLength) / 2;
                int currentX;
                char borderChar = '\u2550';
                for (currentX = 0; currentX < lineSegmentWidth; currentX++)
                {
                    context.RenderChar(new Vector2(currentX + 1, y), borderChar);
                }
                context.RenderString(new Vector2(currentX + 1, y), title);
                currentX += title.Length;
                if (title.Length < titleLength)
                {
                    context.RenderChar(new Vector2(currentX + 1, y), borderChar);
                    currentX++;
                }
                lineSegmentWidth += currentX + 1;
                for (; currentX < lineSegmentWidth; currentX++)
                {
                    context.RenderChar(new Vector2(currentX + 1, y), borderChar);
                }
            }
            else
            {
                base.DrawHorizontalLine(y, context);
            }
        }
        private readonly IMenu mMenu;
    }
}
