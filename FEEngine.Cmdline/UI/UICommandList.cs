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
    /// A list of draw commands to give to the <see cref="Renderer"/>.
    /// </summary>
    public sealed class UICommandList
    {
        public struct DrawCommand
        {
            public Vector Position;
            public char Data;
            public ConsoleColor Color;
        }

        public struct OffsetClipPair
        {
            public Vector Offset, Clip;
        }

        public UICommandList()
        {
            mBuffer = new List<DrawCommand?>();
            OffsetClipStack = new Stack<OffsetClipPair>();
        }

        /// <summary>
        /// A set of <see cref="OffsetClipPair"/>s to control how characters are rendered.
        /// </summary>
        public Stack<OffsetClipPair> OffsetClipStack { get; }

        private OffsetClipPair GetOffsetClipData()
        {
            var data = new OffsetClipPair
            {
                Offset = (0, 0),
                Clip = (-1, -1)
            };

            var array = OffsetClipStack.ToArray();
            for (int i = array.Length - 1; i >= 0; i--)
            {
                var pair = array[i];
                data.Offset += pair.Offset;

                Vector clipWithOffset = data.Offset + pair.Clip;
                if (data.Clip.X > clipWithOffset.X || data.Clip.X < 0)
                {
                    data.Clip.X = clipWithOffset.X;
                }

                if (data.Clip.Y > clipWithOffset.Y || data.Clip.Y < 0)
                {
                    data.Clip.Y = clipWithOffset.Y;
                }
            }

            data.Clip -= data.Offset;
            return data;
        }

        /// <summary>
        /// Pushes a draw command to the buffer.
        /// </summary>
        /// <param name="position">The position at which to draw.</param>
        /// <param name="character">The character to draw.</param>
        /// <param name="color">The color to draw it with. Default resets.</param>
        /// <returns>The index of the pushed command, or -1 on failure.</returns>
        public int Push(Vector position, char character, ConsoleColor color = ConsoleColor.Black)
        {
            if (position.X < 0 || position.Y < 0)
            {
                return -1;
            }

            OffsetClipPair pair = GetOffsetClipData();
            if (position.X >= pair.Clip.X && pair.Clip.X >= 0)
            {
                return -1;
            }

            if (position.Y >= pair.Clip.Y && pair.Clip.Y >= 0)
            {
                return -1;
            }

            var data = new DrawCommand
            {
                Position = position + pair.Offset,
                Data = character,
                Color = color
            };

            int index = mBuffer.Count;
            mBuffer.Add(data);
            return index;
        }

        /// <summary>
        /// Removes a command from the buffer. Does not move other commands in the queue.
        /// </summary>
        /// <param name="index">
        /// The index at which the command is located.
        /// Returned from <see cref="Push(Vector, char, ConsoleColor)"/>
        /// </param>
        /// <returns>If the command was successfully removed.</returns>
        public bool Erase(int index)
        {
            if (index < 0 || index >= mBuffer.Count)
            {
                return false;
            }

            if (mBuffer[index] != null)
            {
                mBuffer[index] = null;
                return true;
            }

            return false;
        }

        /// <summary>
        /// An event on which to alter the set of commands executed.
        /// Indices returned from <see cref="Push(Vector, char, ConsoleColor)"/> may not match up.
        /// </summary>
        public event Action<List<DrawCommand>>? OnExecute;

        /// <summary>
        /// Executes all commands in the buffer.
        /// </summary>
        public void Execute()
        {
            var commands = new List<DrawCommand>();
            foreach (var command in mBuffer)
            {
                if (command != null)
                {
                    commands.Add(command.Value);
                }
            }

            OnExecute?.Invoke(commands);
            foreach (var command in commands)
            {
                Renderer.Draw(command.Position, command.Data, command.Color);
            }
        }

        /// <summary>
        /// Clears the buffer.
        /// </summary>
        public void Clear() => mBuffer.Clear();

        private readonly List<DrawCommand?> mBuffer;
    }
}
