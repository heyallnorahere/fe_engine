﻿/*
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
        private struct RenderedCell
        {
            public char Data;
            public ConsoleColor Color;
        }

        public MapView()
        {
            mCursorPos = (0, 0);
            mAvailableSize = (0, 0);
        }

        public void Render(UICommandList commandList)
        {
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
            int cursorBufferIndex = GetBufferIndex(renderedCursorPos);
            buffer[cursorBufferIndex] = new RenderedCell
            {
                Data = 'v',
                Color = ConsoleColor.White
            };

            var map = Program.Instance.Map;
            foreach (IUnit unit in map.Units)
            {
                Vector unitScreenPos = ToScreenSpace(unit.Position);
                int bufferIndex = GetBufferIndex(unitScreenPos);

                if (buffer[bufferIndex].Data != ' ')
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

            for (int y = 0; y < mAvailableSize.Y; y++)
            {
                for (int x = 0; x < mAvailableSize.X; x++)
                {
                    Vector screenPos = (x, y);
                    int bufferIndex = GetBufferIndex(screenPos);

                    var cellData = buffer[bufferIndex];
                    commandList.Push(screenPos, cellData.Data, cellData.Color);
                }
            }
        }

        private Vector ToScreenSpace(Vector virtualPos) => virtualPos * 2 + 1;
        private int GetBufferIndex(Vector screenPos) => screenPos.Y * mAvailableSize.X + screenPos.X;

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

                mCursorPos = value;
            }
        }

        private Vector mCursorPos;

        public void SetSize(Vector size) => mAvailableSize = size;
        private Vector mAvailableSize;
    }
}