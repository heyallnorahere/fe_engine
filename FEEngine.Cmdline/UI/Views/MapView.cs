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

using System;

namespace FEEngine.Cmdline.UI.Views
{
    public sealed class MapView : IView
    {
        public MapView()
        {
            mCursorPos = (0, 0);
            mLastCursorPos = null;
        }

        public void Render(UICommandList commandList)
        {
            if (mLastCursorPos != null)
            {
                var lastRenderedCursorPos = ToScreenSpace(mLastCursorPos.Value) - (0, 1);
                commandList.Push(lastRenderedCursorPos, ' ');

                mLastCursorPos = null;
            }

            var renderedCursorPos = ToScreenSpace(mCursorPos) - (0, 1);
            commandList.Push(renderedCursorPos, 'v');
        }

        private Vector ToScreenSpace(Vector virtualPos) => virtualPos * 2 + 1;
        public Vector MinSize => ToScreenSpace(Program.Instance.Map.Size);

        public Vector CursorPos
        {
            get => mCursorPos;
            set
            {
                var map = Program.Instance.Map;
                if (map.IsOutOfBounds(value))
                {
                    throw new ArgumentException("The specified point is out of bounds!");
                }

                if (value != mCursorPos)
                {
                    if (mLastCursorPos == null)
                    {
                        mLastCursorPos = mCursorPos;
                    }

                    mCursorPos = value;
                }
            }
        }

        private Vector mCursorPos;
        private Vector? mLastCursorPos;

        public void SetSize(Vector size) => mAvailableSize = size;
        private Vector mAvailableSize;
    }
}
