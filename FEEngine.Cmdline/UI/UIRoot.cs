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
        public UIRoot(Vector canvasSize)
        {
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
            if (Node == null)
            {
                return;
            }

            mCommandList.OffsetClipStack.Push(new UICommandList.OffsetClipPair
            {
                Offset = (0, 0),
                Clip = CanvasSize
            });

            Node.SetSize(CanvasSize);
            Node.Render(mCommandList);

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
        public IView? Node { get; set; }

        private readonly UICommandList mCommandList;
    }
}
