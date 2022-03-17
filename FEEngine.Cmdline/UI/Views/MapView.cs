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
using System.Collections.Generic;

namespace FEEngine.Cmdline.UI.Views
{
    public sealed class MapView : IView
    {
        private struct RenderedCell
        {
            public char Data;
            public ConsoleColor Color;
        }

        public MapView()
        {
            mCursorPos = (0, 0);
            mViewOffset = (0, 0);
            mAvailableSize = (0, 0);
        }
        
        private void InvalidateViewOffset()
        {
            Vector currentCursorRenderPos = ToScreenSpace(mCursorPos);
            if (currentCursorRenderPos.X >= 0 && currentCursorRenderPos.X < mAvailableSize.X &&
                currentCursorRenderPos.Y >= 0 && currentCursorRenderPos.Y < mAvailableSize.Y)
            {
                return;
            }

            Vector absRenderPos = ToScreenSpace(mCursorPos, (0, 0));
            if (currentCursorRenderPos.X < 0)
            {
                mViewOffset.X = absRenderPos.X - 1;
            }

            if (currentCursorRenderPos.Y < 0)
            {
                mViewOffset.Y = absRenderPos.Y - 1;
            }

            if (currentCursorRenderPos.X >= mAvailableSize.X)
            {
                mViewOffset.X = absRenderPos.X + 1 - mAvailableSize.X;
            }

            if (currentCursorRenderPos.Y >= mAvailableSize.Y)
            {
                mViewOffset.Y = absRenderPos.Y + 1 - mAvailableSize.Y;
            }
        }

        public void Render(UICommandList commandList)
        {
            InvalidateViewOffset();

            int bufferSize = mAvailableSize.X * mAvailableSize.Y;
            var buffer = new RenderedCell[bufferSize];

            for (int i = 0; i < bufferSize; i++)
            {
                buffer[i] = new RenderedCell
                {
                    Data = ' ',
                    Color = ConsoleColor.White,
                };
            }

            var renderedCursorPos = ToScreenSpace(mCursorPos) - (0, 1);
            int bufferIndex = GetBufferIndex(renderedCursorPos);
            buffer[bufferIndex] = new RenderedCell
            {
                Data = 'v',
                Color = ConsoleColor.White
            };

            var map = Program.Instance.Map;
            var unitPositions = new HashSet<Vector>();
            foreach (IUnit unit in map.Units)
            {
                var unitScreenPos = ToScreenSpace(unit.Position);
                if (unitScreenPos.X < 0 || unitScreenPos.X >= mAvailableSize.X ||
                    unitScreenPos.Y < 0 || unitScreenPos.Y >= mAvailableSize.Y)
                {
                    continue;
                }

                bufferIndex = GetBufferIndex(unitScreenPos);
                var data = buffer[bufferIndex];
                if (data.Data != ' ')
                {
                    throw new ArgumentException("There are two units of the same position!");
                }

                if (unit.UserData is UnitUserData userData)
                {
                    var color = userData.Allegiance switch
                    {
                        Allegiance.Player => ConsoleColor.Blue,
                        Allegiance.Enemy => ConsoleColor.Red,
                        _ => throw new ArgumentException("Invalid allegiance!")
                    };

                    var weaponType = unit.EquippedWeapon?.WeaponData?.Type ?? WeaponType.None;
                    char character = weaponType switch
                    {
                        WeaponType.None => 'n',
                        WeaponType.Sword => 'S',
                        WeaponType.Axe => 'A',
                        WeaponType.Lance => 'L',
                        WeaponType.Bow => 'B',
                        WeaponType.Gauntlets => 'G',
                        WeaponType.Magic => 'M',
                        _ => throw new ArgumentException("Invalid weapon type!")
                    };

                    unitPositions.Add(unit.Position);
                    buffer[bufferIndex] = new RenderedCell
                    {
                        Data = character,
                        Color = color
                    };
                }
                else
                {
                    throw new InvalidOperationException("Invalid unit!");
                }
            }

            for (int y = 0; y < map.Size.Y; y++)
            {
                for (int x = 0; x < map.Size.X; x++)
                {
                    Vector position = (x, y);
                    if (unitPositions.Contains(position))
                    {
                        continue;
                    }

                    Vector screenPos = ToScreenSpace(position);
                    if (screenPos.X < 0 || screenPos.X >= mAvailableSize.X ||
                        screenPos.Y < 0 || screenPos.Y >= mAvailableSize.Y)
                    {
                        continue;
                    }

                    bufferIndex = GetBufferIndex(screenPos);
                    buffer[bufferIndex] = new RenderedCell
                    {
                        Data = '-',
                        Color = ConsoleColor.White
                    };
                }
            }

            for (int y = 0; y < mAvailableSize.Y; y++)
            {
                for (int x = 0; x < mAvailableSize.X; x++)
                {
                    Vector screenPos = (x, y);
                    bufferIndex = GetBufferIndex(screenPos);

                    var cellData = buffer[bufferIndex];
                    commandList.Push(screenPos, cellData.Data, cellData.Color);
                }
            }
        }

        private Vector ToScreenSpace(Vector virtualPos, Vector? viewOffset = null)
        {
            Vector result = virtualPos * 2 + 1;
            result -= viewOffset ?? mViewOffset;

            return result;
        }

        private int GetBufferIndex(Vector screenPos) => screenPos.Y * mAvailableSize.X + screenPos.X;
        public Vector MinSize => (10, 10);

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

                mCursorPos = value;
            }
        }

        private Vector mCursorPos;
        private Vector mViewOffset;

        public void SetSize(Vector size) => mAvailableSize = size;
        private Vector mAvailableSize;
    }
}
