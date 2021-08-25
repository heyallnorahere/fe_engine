using System.Collections.Generic;

namespace FEEngine
{
    /// <summary>
    /// A renderable object to help position other renderable objects
    /// </summary>
    public class BorderLayout : IRenderable
    {
        public enum Alignment
        {
            Top,
            Bottom,
            Left,
            Right
        }
        private struct ChildAlignmentPair
        {
            public IRenderable Child { get; set; }
            public Alignment Alignment { get; set; }
        }
        public BorderLayout()
        {
            mChildren = new();
            Center = null;
            mSize = new Vector2(0);
        }
        public Vector2 MinSize
        {
            get
            {
                var size = new Vector2(0);
                foreach (ChildAlignmentPair pair in mChildren)
                {
                    Vector2 minSize = pair.Child.MinSize;
                    if (pair.Alignment == Alignment.Left || pair.Alignment == Alignment.Right)
                    {
                        size.X += minSize.X;
                        if (size.Y < minSize.Y)
                        {
                            size.Y = minSize.Y;
                        }
                    }
                    if (pair.Alignment == Alignment.Top || pair.Alignment == Alignment.Bottom)
                    {
                        size.Y += minSize.Y;
                        if (size.Y < minSize.Y)
                        {
                            size.Y = minSize.Y;
                        }
                    }
                }
                size += Center?.MinSize ?? new Vector2(0);
                return size;
            }
        }
        public void Render(RenderContext context)
        {
            var sizeRemaining = new Vector2(mSize);
            var originRemaining = new Vector2(0);
            foreach (ChildAlignmentPair pair in mChildren)
            {
                var childSize = new Vector2(0);
                var childPos = new Vector2(0);
                var minSize = pair.Child.MinSize;
                switch (pair.Alignment)
                {
                    case Alignment.Bottom:
                        childPos.X = originRemaining.X;
                        childPos.Y = originRemaining.Y;
                        childSize.X = sizeRemaining.X;
                        childSize.Y = minSize.Y;
                        originRemaining.Y += childSize.Y;
                        sizeRemaining.Y -= childSize.Y;
                        break;
                    case Alignment.Top:
                        childPos.X = originRemaining.X;
                        childPos.Y = mSize.Y - minSize.Y;
                        childSize.X = sizeRemaining.X;
                        childSize.Y = minSize.Y;
                        sizeRemaining.Y -= childSize.Y;
                        break;
                    case Alignment.Left:
                        childPos.X = originRemaining.X;
                        childPos.Y = originRemaining.Y;
                        childSize.X = minSize.X;
                        childSize.Y = sizeRemaining.Y;
                        originRemaining.X += childSize.X;
                        sizeRemaining.X -= childSize.X;
                        break;
                    case Alignment.Right:
                        childPos.X = mSize.X - minSize.X;
                        childPos.Y = originRemaining.Y;
                        childSize.X = minSize.X;
                        childSize.Y = sizeRemaining.Y;
                        sizeRemaining.X -= childSize.X;
                        break;
                }
                pair.Child.SetSize(childSize);
                context.PushPair(new RenderContext.OffsetClipPair()
                {
                    Offset = childPos,
                    Clip = childSize
                });
                pair.Child.Render(context);
                context.PopPair();
            }
            if (Center != null)
            {
                Center.SetSize(sizeRemaining);
                context.PushPair(new RenderContext.OffsetClipPair()
                {
                    Offset = originRemaining,
                    Clip = sizeRemaining
                });
                Center.Render(context);
            }
            context.PopPair();
        }
        public void SetSize(Vector2 size)
        {
            mSize = size;
        }
        /// <summary>
        /// Adds an object to the <see cref="BorderLayout"/> to render after the existing children with the remaining space
        /// </summary>
        /// <param name="renderable">The object to add</param>
        /// <param name="alignment">Where to place the object</param>
        public void AddChild(IRenderable renderable, Alignment alignment)
        {
            mChildren.Add(new ChildAlignmentPair()
            {
                Child = renderable,
                Alignment = alignment
            });
        }
        public IRenderable? Center { get; set; }
        private Vector2 mSize;
        private readonly List<ChildAlignmentPair> mChildren;
    }
}
