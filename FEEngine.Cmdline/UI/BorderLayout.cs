/*
   Copyright 2022 Nora Beda

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System.Collections.Generic;

namespace FEEngine.Cmdline.UI
{
    /// <summary>
    /// A border layout allows child <see cref="IView"/>s to be rendered in a tile-like format.
    /// </summary>
    public sealed class BorderLayout : IView
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
            public IView View;
            public Alignment Alignment;
        }

        public BorderLayout()
        {
            Center = null;
            mChildren = new List<ChildAlignmentPair>();
            mSize = (0, 0);
        }

        // todo: border characters
        public void Render(UICommandList commandList)
        {
            Vector sizeRemaining = mSize;
            Vector originRemaining = (0, 0);

            foreach (var child in mChildren)
            {
                Vector childSize = (0, 0);
                Vector childPos = (0, 0);

                var minSize = child.View.MinSize;
                switch (child.Alignment)
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

                commandList.OffsetClipStack.Push(new UICommandList.OffsetClipPair
                {
                    Offset = childPos,
                    Clip = childSize
                });

                child.View.SetSize(childSize);
                child.View.Render(commandList);

                commandList.OffsetClipStack.Pop();
            }

            if (Center != null)
            {
                commandList.OffsetClipStack.Push(new UICommandList.OffsetClipPair
                {
                    Offset = originRemaining,
                    Clip = sizeRemaining
                });

                Center.SetSize(sizeRemaining);
                Center.Render(commandList);

                commandList.OffsetClipStack.Pop();
            }
        }

        public Vector MinSize
        {
            get
            {
                Vector size = (0, 0);
                foreach (var child in mChildren)
                {
                    Vector minSize = child.View.MinSize;
                    if (child.Alignment == Alignment.Left || child.Alignment == Alignment.Right)
                    {
                        size.X += minSize.X;
                        if (size.Y < minSize.Y)
                        {
                            size.Y = minSize.Y;
                        }
                    }
                    else if (child.Alignment == Alignment.Top || child.Alignment == Alignment.Bottom)
                    {
                        size.Y += minSize.Y;
                        if (size.X < minSize.X)
                        {
                            size.X = minSize.X;
                        }
                    }
                }

                if (Center != null)
                {
                    size += Center.MinSize;
                }

                return size;
            }
        }

        public void SetSize(Vector size) => mSize = size;

        /// <summary>
        /// Adds a child to the layout.
        /// </summary>
        /// <param name="view">The child to add.</param>
        /// <param name="alignment">The alignment to be given.</param>
        public void AddChild(IView view, Alignment alignment)
        {
            mChildren.Add(new ChildAlignmentPair
            {
                View = view,
                Alignment = alignment
            });
        }

        public IView? Center { get; set; }

        private Vector mSize;
        private readonly List<ChildAlignmentPair> mChildren;
    }
}
