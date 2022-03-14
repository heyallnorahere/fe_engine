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

namespace FEEngine.Cmdline.UI
{
    /// <summary>
    /// The root of the UI system.
    /// </summary>
    public sealed class UIRoot
    {
        private sealed class View : IView
        {
            public View()
            {
                Child = null;
                mSize = (0, 0);
            }

            public IView? Child { get; set; }

            public void Render(UICommandList commandList)
            {
                var push = (Vector position) => commandList.Push(position, BorderLayout.BorderCharacter);
                for (int x = 0; x < mSize.X; x++)
                {
                    push((x, 0));
                    push((x, mSize.Y - 1));
                }

                for (int y = 1; y < mSize.Y - 1; y++)
                {
                    push((0, y));
                    push((mSize.X - 1, y));
                }

                if (Child != null)
                {
                    Vector availableSize = mSize - 2;
                    commandList.OffsetClipStack.Push(new UICommandList.OffsetClipPair
                    {
                        Offset = (1, 1),
                        Clip = availableSize
                    });

                    Child.SetSize(availableSize);
                    Child.Render(commandList);

                    commandList.OffsetClipStack.Pop();
                }
            }

            public Vector MinSize
            {
                get
                {
                    Vector minSize = (2, 2);
                    if (Child != null)
                    {
                        minSize += Child.MinSize;
                    }

                    return minSize;
                }
            }

            public void SetSize(Vector size) => mSize = size;
            private Vector mSize;
        }

        public UIRoot(Vector canvasSize)
        {
            mView = new View();
            mCommandList = new UICommandList();
            // todo: some kind of callback to create borders

            CanvasSize = canvasSize;
            Node = null;
        }

        /// <summary>
        /// Renders the UI.
        /// </summary>
        public void Render()
        {
            mCommandList.OffsetClipStack.Push(new UICommandList.OffsetClipPair
            {
                Offset = (0, 0),
                Clip = CanvasSize
            });

            mView.SetSize(CanvasSize);
            mView.Render(mCommandList);

            mCommandList.OffsetClipStack.Pop();
            mCommandList.Execute();
            mCommandList.Clear();
        }

        /// <summary>
        /// The size of the canvas.
        /// </summary>
        public Vector CanvasSize { get; }

        /// <summary>
        /// The root node
        /// </summary>
        public IView? Node
        {
            get => mView.Child;
            set => mView.Child = value;
        }

        private readonly View mView;
        private readonly UICommandList mCommandList;
    }
}
