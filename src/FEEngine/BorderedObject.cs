using FEEngine.Math;

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
            mRenderSize = new Vec2I(0);
        }
        public IVec2<int> MinSize => MathUtil.AddVectors(mChild.MinSize, new Vec2I(2));
        public void Render(RenderContext context)
        {
            DrawBorder(context);
            IVec2<int> childRenderSize = MathUtil.SubVectors(mRenderSize, new Vec2I(2));
            mChild.SetSize(childRenderSize);
            context.PushPair(new RenderContext.OffsetClipPair()
            {
                Offset = new Vec2I(1),
                Clip = childRenderSize
            });
            mChild.Render(context);
            context.PopPair();
        }
        protected virtual void DrawHorizontalLine(int y, RenderContext context)
        {
            for (int x = 0; x < mRenderSize.X - 2; x++)
            {
                context.RenderChar(new Vec2I(x + 1, y), '\u2550');
            }
        }
        private void DrawVerticalLine(int x, RenderContext context)
        {
            for (int y = 0; y < mRenderSize.Y - 2; y++)
            {
                context.RenderChar(new Vec2I(x, y + 1), '\u2551');
            }
        }
        private void DrawCorners(RenderContext context)
        {
            context.RenderChar(new Vec2I(0), '\u255A');
            context.RenderChar(new Vec2I(mRenderSize.X - 1, 0), '\u255D');
            context.RenderChar(new Vec2I(0, mRenderSize.Y - 1), '\u2554');
            context.RenderChar(new Vec2I(mRenderSize.X - 1, mRenderSize.Y - 1), '\u2557');
        }
        private void DrawBorder(RenderContext context)
        {
            DrawHorizontalLine(mRenderSize.Y - 1, context);
            DrawHorizontalLine(0, context);
            DrawVerticalLine(0, context);
            DrawVerticalLine(mRenderSize.X - 1, context);
            DrawCorners(context);
        }
        public void SetSize(IVec2<int> size)
        {
            mRenderSize = size;
        }
        private IVec2<int> mRenderSize;
        protected IVec2<int> RenderSize => mRenderSize;
        private readonly IRenderable mChild;
    }
}
