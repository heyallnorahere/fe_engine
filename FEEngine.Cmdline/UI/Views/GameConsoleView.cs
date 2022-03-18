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

            mSize = (0, 0);
        }

        private void OnInput(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.Enter:
                    if (mCurrentCommand.Length > 0)
                    {
                        if (mSubmittedCommand.Length > 0)
                        {
                            throw new ArgumentException("Cannot submit two commands in one frame!");
                        }

                        mSubmittedCommand = mCurrentCommand;
                        mCurrentCommand = string.Empty;
                    }
                    break;
                case ConsoleKey.Backspace:
                    if (mCurrentCommand.Length > 0)
                    {
                        mCurrentCommand = mCurrentCommand[..^1];
                    }
                    break;
                default:
                    mCurrentCommand += keyInfo.KeyChar;
                    break;
            }
        }

        public void Render(UICommandList commandList)
        {
            if (mSubmittedCommand.Length > 0)
            {
                mLines.Enqueue($"> {mSubmittedCommand}");

                var lines = GameConsole.Execute(mSubmittedCommand);
                foreach (var line in lines)
                {
                    mLines.Enqueue(line);
                }

                mSubmittedCommand = string.Empty;
            }

            var lineArray = mLines.ToArray();
            var wrappedLines = new List<string>();

            foreach (string line in lineArray)
            {
                var wrapped = Wrap(line);
                wrappedLines.AddRange(wrapped);
            }

            int bufferSize = mSize.X * mSize.Y;
            var buffer = new char[bufferSize];

            for (int i = 0; i < bufferSize; i++)
            {
                buffer[i] = ' ';
            }

            int spaceAvailable = mSize.Y - 1;
            string[] wrappedLinesArray = wrappedLines.ToArray();

            string[] renderedLines;
            if (wrappedLinesArray.Length > spaceAvailable)
            {
                renderedLines = wrappedLinesArray[^4..];
            }
            else
            {
                renderedLines = wrappedLinesArray;
            }

            var getBufferIndex = (Vector pos) => pos.Y * mSize.X + pos.X;
            for (int y = 0; y < renderedLines.Length; y++)
            {
                string line = renderedLines[y];
                for (int x = 0; x < line.Length; x++)
                {
                    Vector pos = (x, y);
                    char character = line[x];

                    int bufferIndex = getBufferIndex(pos);
                    buffer[bufferIndex] = character;
                }
            }

            Vector commandPromptPos = (0, mSize.Y - 1);
            int commandPromptIndex = getBufferIndex(commandPromptPos);
            buffer[commandPromptIndex] = '>';

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

        public Vector MinSize => (15, 5);

        public void SetSize(Vector size) => mSize = size;
        private Vector mSize;

        private readonly Queue<string> mLines;
        private string mSubmittedCommand, mCurrentCommand;
    }
}
