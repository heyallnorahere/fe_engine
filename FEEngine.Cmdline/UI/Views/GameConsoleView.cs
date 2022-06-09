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
    public sealed class GameConsoleView : IView
    {
        public GameConsoleView()
        {
            Renderer.OnInput += OnInput;

            mCurrentCommand = string.Empty;
            mSubmittedCommand = string.Empty;
            mLines = new Queue<string>();
            mVerticalPosition = -1;
            mHorizontalPosition = 0;

            mSize = (0, 0);
        }

        private void OnInput(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    if (mVerticalPosition < 0)
                    {
                        mVerticalPosition = 1;
                    }
                    else if (mVerticalPosition < mLines.Count - 2)
                    {
                        mVerticalPosition++;
                    }

                    break;
                case ConsoleKey.DownArrow:
                    if (mVerticalPosition > 0)
                    {
                        mVerticalPosition--;
                    }

                    break;
                case ConsoleKey.LeftArrow:
                    if (mHorizontalPosition > 0)
                    {
                        mHorizontalPosition--;
                    }

                    break;
                case ConsoleKey.RightArrow:
                    if (mHorizontalPosition < mCurrentCommand.Length)
                    {
                        mHorizontalPosition++;
                    }

                    break;
                case ConsoleKey.Enter:
                    if (mCurrentCommand.Length > 0)
                    {
                        if (mSubmittedCommand.Length > 0)
                        {
                            throw new ArgumentException("Cannot submit two commands in one frame!");
                        }

                        mSubmittedCommand = mCurrentCommand;
                        mCurrentCommand = string.Empty;
                        mVerticalPosition = -1;
                        mHorizontalPosition = 0;
                    }

                    break;
                case ConsoleKey.Backspace:
                    if (mHorizontalPosition > 0)
                    {
                        mCurrentCommand = mCurrentCommand.Remove(--mHorizontalPosition, 1);
                    }

                    break;
                default:
                    if (mVerticalPosition >= 0)
                    {
                        mVerticalPosition = -1;
                    }

                    char keyChar = keyInfo.KeyChar;
                    mCurrentCommand = mCurrentCommand.Insert(mHorizontalPosition++, $"{keyChar}");

                    break;
            }
        }

        public void Render(UICommandList commandList)
        {
            if (mSubmittedCommand.Length > 0)
            {
                mLines.Enqueue($"> {mSubmittedCommand}");

                var output = GameConsole.Execute(mSubmittedCommand);
                foreach (var line in output)
                {
                    var wrappedLines = Wrap(line);
                    foreach (var wrappedLine in wrappedLines)
                    {
                        mLines.Enqueue(wrappedLine);
                    }

                    if (mVerticalPosition >= 0)
                    {
                        mVerticalPosition += wrappedLines.Count;
                    }
                }

                mSubmittedCommand = string.Empty;
            }

            var lines = mLines.ToArray();
            int bufferSize = mSize.X * mSize.Y;
            var buffer = new char[bufferSize];

            for (int i = 0; i < bufferSize; i++)
            {
                buffer[i] = ' ';
            }


            int spaceAvailable = mSize.Y - 1;
            int verticalPos = mVerticalPosition;

            if (verticalPos < 0)
            {
                verticalPos = 0;
            }

            string[] renderedLines;
            if (lines.Length > spaceAvailable)
            {
                int end = verticalPos;
                int start = end + spaceAvailable;

                if (start >= lines.Length)
                {
                    start = lines.Length - 1;
                }

                renderedLines = lines[^start..^end];
            }
            else
            {
                renderedLines = lines;
            }

            var getBufferIndex = (Vector pos) => pos.Y * mSize.X + pos.X;
            int yOffset = spaceAvailable - renderedLines.Length;
            for (int y = 0; y < renderedLines.Length; y++)
            {
                string line = renderedLines[y];
                for (int x = 0; x < line.Length; x++)
                {
                    Vector pos = (x, y + yOffset);
                    char character = line[x];

                    int bufferIndex = getBufferIndex(pos);
                    buffer[bufferIndex] = character;
                }
            }

            Vector commandPromptPos = (0, mSize.Y - 1);
            int commandPromptIndex = getBufferIndex(commandPromptPos);
            buffer[commandPromptIndex] = '>';

            // todo: use mHorizontalPosition
            spaceAvailable = mSize.X - 3;
            string displayedCommand;

            if (mCurrentCommand.Length > spaceAvailable)
            {
                string commandData =  mCurrentCommand[^(spaceAvailable + 3)..];
                displayedCommand = $"...{commandData}";
            }
            else
            {
                displayedCommand = mCurrentCommand;
            }

            for (int i = 0; i < displayedCommand.Length; i++)
            {
                Vector characterPos = (i + 2, mSize.Y - 1);
                char character = displayedCommand[i];

                int characterBufferIndex = getBufferIndex(characterPos);
                buffer[characterBufferIndex] = character;
            }

            Vector commandCursorPos = (displayedCommand.Length + 2, mSize.Y - 1);
            int commandCursorIndex = getBufferIndex(commandCursorPos);
            buffer[commandCursorIndex] = '\u2588';

            for (int y = 0; y < mSize.Y; y++)
            {
                for (int x = 0; x < mSize.X; x++)
                {
                    Vector pos = (x, y);
                    int bufferIndex = getBufferIndex(pos);

                    char character = buffer[bufferIndex];
                    commandList.Push(pos, character);
                }
            }
        }

        private IReadOnlyList<string> Wrap(string line)
        {
            var lines = new List<string>();
            int lineStart = 0;

            for (int i = 0; i < line.Length; i++)
            {
                int currentLength = i - lineStart;
                if (currentLength > mSize.X)
                {
                    lines.Add(line[lineStart..(i - 1)]);
                    lineStart = i;
                }
            }

            string lastLine = line[lineStart..];
            if (lastLine.Length > 0)
            {
                lines.Add(lastLine);
            }

            return lines;
        }

        public Vector MinSize => (15, 8);

        public void SetSize(Vector size) => mSize = size;
        private Vector mSize;

        private readonly Queue<string> mLines;
        private string mSubmittedCommand, mCurrentCommand;
        private int mVerticalPosition, mHorizontalPosition;
    }
}
