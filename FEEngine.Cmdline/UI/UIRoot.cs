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
            mCommandList.OnExecute += RenderBorders;

            CanvasSize = canvasSize;
            Node = null;
        }

        private const int Left = 0x1;
        private const int Right = 0x2;
        private const int Up = 0x4;
        private const int Down = 0x8;

        private void RenderBorders(List<UICommandList.DrawCommand> drawCommands)
        {
            var borderPositions = new Dictionary<Vector, ConsoleColor>();
            foreach (var command in drawCommands)
            {
                if (command.Data == BorderLayout.BorderCharacter)
                {
                    if (borderPositions.ContainsKey(command.Position))
                    {
                        borderPositions[command.Position] = command.Color;
                    }
                    else
                    {
                        borderPositions.Add(command.Position, command.Color);
                    }
                }
                else if (borderPositions.ContainsKey(command.Position))
                {
                    borderPositions.Remove(command.Position);
                }
            }

            var otherCommands = drawCommands.FindAll(command => !borderPositions.ContainsKey(command.Position));
            drawCommands.Clear();
            drawCommands.AddRange(otherCommands);

            foreach (Vector position in borderPositions.Keys)
            {
                int surroundings = 0;
                var setBitFlag = (Vector delta, int value) =>
                {
                    if (borderPositions.ContainsKey(position + delta))
                    {
                        surroundings |= value;
                    }
                };

                setBitFlag((-1, 0), Left);
                setBitFlag((1, 0), Right);
                setBitFlag((0, -1), Up);
                setBitFlag((0, 1), Down);

                // https://en.wikipedia.org/wiki/List_of_Unicode_characters#Box_Drawing
                char character = surroundings switch
                {
                    Left | Right => '\u2550',
                    Up | Down => '\u2551',
                    Down | Right => '\u2554',
                    Down | Left => '\u2557',
                    Up | Right => '\u255a',
                    Up | Left => '\u255d',
                    Up | Down | Right => '\u2560',
                    Up | Down | Left => '\u2563',
                    Down | Left | Right => '\u2566',
                    Up | Left | Right => '\u2569',
                    Up | Down | Left | Right => '\u256c',
                    _ => throw new ArgumentException("Invalid bit flag combination!")
                };

                drawCommands.Add(new UICommandList.DrawCommand
                {
                    Position = position,
                    Data = character,
                    Color = borderPositions[position]
                });
            }
        }

        /// <summary>
        /// Finds the first instance of the requested view type.
        /// Uses <see cref="BorderLayout.FindView{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of view to search for.</typeparam>
        /// <returns>The found view, or null if the function failed.</returns>
        public T? FindView<T>() where T : IView
        {
            if (Node != null)
            {
                return BorderLayout.GetInternalView<T>(Node);
            }
            else
            {
                return default;
            }
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
