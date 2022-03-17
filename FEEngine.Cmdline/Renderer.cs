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
using System.Collections.Generic;

namespace FEEngine.Cmdline
{
    public static class Renderer
    {
        private struct Character
        {
            public char Data;
            public ConsoleColor Color;
        }

        private static Vector sOriginalSize;
        private static ConsoleColor? sColor;
        private static Dictionary<Vector, Character> mPrintedCharacters;

        static Renderer()
        {
            sOriginalSize = (0, 0);
            sColor = null;
            mPrintedCharacters = new Dictionary<Vector, Character>();

            Console.Clear();
            Console.TreatControlCAsInput = true;
            Console.CursorVisible = false;
        }

        public static event System.Action? OnCtrlC;
        public static event Action<ConsoleKeyInfo>? OnInput;

        public static void HandleInputs()
        {
            while (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);
                if ((key.Modifiers & ConsoleModifiers.Control) != 0 && key.Key == ConsoleKey.C)
                {
                    Console.CursorVisible = true;
                    Console.CursorTop = 0;
                    Console.CursorLeft = 0;

                    Console.ResetColor();
                    Console.Clear();
                    Console.WriteLine("Exiting FEEngine...");

                    if (sOriginalSize != (0, 0) && OperatingSystem.IsWindows())
                    {
                        Console.WindowWidth = sOriginalSize.X;
                        Console.WindowHeight = sOriginalSize.Y;
                    }

                    OnCtrlC?.Invoke();
                }
                else
                {
                    OnInput?.Invoke(key);
                }
            }
        }

        public static void Clear()
        {
            Console.Clear();
            mPrintedCharacters.Clear();
        }

        public static bool Draw(Vector position, char character, ConsoleColor color = ConsoleColor.White)
        {
            if (position.X < 0 || position.Y < 0)
            {
                return false;
            }

            bool positionPrinted = mPrintedCharacters.ContainsKey(position);
            if (positionPrinted)
            {
                var data = mPrintedCharacters[position];
                if (character == data.Data && color == data.Color)
                {
                    return true;
                }
            }
            else if (character == ' ')
            {
                return true;
            }

            if (OperatingSystem.IsWindows())
            {
                Vector requiredSize = position + 1;
                Vector currentSize = (Console.WindowWidth, Console.WindowHeight);

                if (requiredSize.X > currentSize.X ||
                    requiredSize.Y > currentSize.Y)
                {
                    if (requiredSize.X > currentSize.X)
                    {
                        Console.WindowWidth = requiredSize.X;
                    }

                    if (requiredSize.Y > currentSize.Y)
                    {
                        Console.WindowHeight = requiredSize.Y;
                    }

                    if (sOriginalSize == (0, 0))
                    {
                        sOriginalSize = currentSize;
                    }
                }
            }

            Console.CursorLeft = position.X;
            Console.CursorTop = position.Y;

            if (color != sColor)
            {
                if (color != ConsoleColor.Black)
                {
                    Console.ForegroundColor = color;
                }
                else
                {
                    Console.ResetColor();
                }

                sColor = color;
            }

            var characterData = new Character
            {
                Data = character,
                Color = color
            };

            if (positionPrinted)
            {
                mPrintedCharacters[position] = characterData;
            }
            else
            {
                mPrintedCharacters.Add(position, characterData);
            }

            Console.Write(character);
            return true;
        }
    }
}
