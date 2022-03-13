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
    /// A renderable view.
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// Renders the view.
        /// </summary>
        public void Render(UICommandList commandList);

        /// <summary>
        /// The minimum size required to render this view.
        /// </summary>
        public Vector MinSize { get; }

        /// <summary>
        /// Communicates to the view how much space it has to render.
        /// </summary>
        /// <param name="size">The available space.</param>
        public void SetSize(Vector size);
    }
}
