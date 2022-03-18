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

namespace FEEngine
{
    public static class Utilities
    {
        /// <summary>
        /// Tokenizes the command.
        /// </summary>
        /// <param name="command">The original command string.</param>
        /// <returns>Tokens parsed from the command.</returns>
        /// <exception cref="ArgumentException" />
        public static string[] Tokenize(this string command)
        {
            string[] sections = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            string? currentToken = null;
            var completedTokens = new List<string>();

            foreach (string section in sections)
            {
                bool containsQuote = false;
                bool escapeNextCharacter = false;

                string token = string.Empty;
                for (int i = 0; i < section.Length; i++)
                {
                    char character = section[i];

                    switch (character)
                    {
                        case '"':
                            if (escapeNextCharacter)
                            {
                                token += character;
                                escapeNextCharacter = false;
                            }
                            else
                            {
                                containsQuote = !containsQuote;
                            }
                            break;
                        case '\\':
                            if (i == section.Length - 1)
                            {
                                containsQuote = true;
                            }
                            else if (!escapeNextCharacter)
                            {
                                escapeNextCharacter = true;
                            }
                            else
                            {
                                escapeNextCharacter = false;
                                token += character;
                            }
                            break;
                        default:
                            if (escapeNextCharacter)
                            {
                                throw new ArgumentException($"cannot escape character: {character}");
                            }
                            else
                            {
                                token += character;
                            }
                            break;
                    }
                }

                bool beganToken = false;
                if (currentToken == null && containsQuote)
                {
                    currentToken = string.Empty;
                    beganToken = true;
                }

                if (currentToken != null)
                {
                    currentToken += " " + token;
                }
                else
                {
                    completedTokens.Add(token);
                }

                if (containsQuote && !beganToken && currentToken != null)
                {
                    completedTokens.Add(currentToken);
                    currentToken = null;
                }
            }

            if (currentToken != null)
            {
                completedTokens.Add(currentToken);
            }

            return completedTokens.ToArray();
        }
    }
}
