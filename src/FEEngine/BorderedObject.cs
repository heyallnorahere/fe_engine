namespace FEEngine
{
    /// <summary>
    /// A renderable object that takes child <see cref="IRenderable"/> objects, and renders them with a border
    /// </summary>
    public class BorderedObject : IRenderable
    {
        public BorderedObject(IRenderable child)
        {
            mChild = child;
            mRenderSize = new Vector2(0);
        }
        public Vector2 MinSize => mChild.MinSize + new Vector2(2);
        public void Render(RenderContext context)
        {
            DrawBorder(context);
            Vector2 childRenderSize = mRenderSize - new Vector2(2);
            mChild.SetSize(childRenderSize);
            context.PushPair(new RenderContext.OffsetClipPair()
            {
                Offset = new Vector2(1),
                Clip = childRenderSize
            });
            mChild.Render(context);
            context.PopPair();
        }
        protected virtual void DrawHorizontalLine(int y, RenderContext context)
        {
            for (int x = 0; x < mRenderSize.X - 2; x++)
            {
                context.RenderChar(new Vector2(x + 1, y), '\u2550');
            }
        }
        private void DrawVerticalLine(int x, RenderContext context)
        {
            for (int y = 0; y < mRenderSize.Y - 2; y++)
            {
                context.RenderChar(new Vector2(x, y + 1), '\u2551');
            }
        }
        private void DrawCorners(RenderContext context)
        {
            context.RenderChar(new Vector2(0), '\u255A');
            context.RenderChar(new Vector2(mRenderSize.X - 1, 0), '\u255D');
            context.RenderChar(new Vector2(0, mRenderSize.Y - 1), '\u2554');
            context.RenderChar(new Vector2(mRenderSize.X - 1, mRenderSize.Y - 1), '\u2557');
        }
        private void DrawBorder(RenderContext context)
        {
            DrawHorizontalLine(mRenderSize.Y - 1, context);
            DrawHorizontalLine(0, context);
            DrawVerticalLine(0, context);
            DrawVerticalLine(mRenderSize.X - 1, context);
            DrawCorners(context);
        }
        public void SetSize(Vector2 size)
        {
            mRenderSize = size;
        }
        private Vector2 mRenderSize;
        protected Vector2 RenderSize => mRenderSize;
        private readonly IRenderable mChild;
    }
}
