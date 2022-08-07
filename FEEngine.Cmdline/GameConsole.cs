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
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace FEEngine.Cmdline
{
    /// <summary>
    /// Used as a delegate in <see cref="IConsoleCommand"/>.
    /// </summary>
    /// <param name="args">The arguments that were passed.</param>
    public delegate void ConsoleCommandExecutionCallback(string[] args, TextWriter output);

    /// <summary>
    /// A command to be invoked via <see cref="GameConsole"/>.
    /// </summary>
    public interface IConsoleCommand
    {
        /// <summary>
        /// A list of subcommands and their names
        /// </summary>
        public IReadOnlyDictionary<string, IConsoleCommand> Subcommands { get; }

        /// <summary>
        /// Executes the command.
        /// </summary>
        public ConsoleCommandExecutionCallback? Execute { get; }
    }

    /// <summary>
    /// Registers a command with the <see cref="GameConsole"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class RegisteredCommandAttribute : Attribute
    {
        public RegisteredCommandAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The name of this command.
        /// </summary>
        public string Name { get; }
    }

    /// <summary>
    /// A console that commands are to be issued to.
    /// </summary>
    public static class GameConsole
    {
        private static readonly Dictionary<string, IConsoleCommand> sBaseCommands;
        public static IEnumerable<string> RegisteredCommands => sBaseCommands.Keys;

        static GameConsole()
        {
            sBaseCommands = new Dictionary<string, IConsoleCommand>();

            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();

            foreach (var type in types)
            {
                var interfaces = new List<Type>(type.GetInterfaces());
                if (!interfaces.Contains(typeof(IConsoleCommand)))
                {
                    continue;
                }

                var attribute = type.GetCustomAttribute<RegisteredCommandAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                string commandName = attribute.Name;
                var match = Regex.Match(commandName, @"[a-z-]+");
                
                if (!match.Success || match.Length != commandName.Length)
                {
                    continue;
                }

                if (sBaseCommands.ContainsKey(commandName))
                {
                    throw new ArgumentException($"Command \"{commandName}\" already exists!");
                }

                var constructor = type.GetConstructor(Array.Empty<Type>());
                object? instance = constructor?.Invoke(null);

                if (instance is IConsoleCommand command)
                {
                    sBaseCommands.Add(commandName, command);
                }
            }
        }

        private class ConsoleWriter : TextWriter
        {
            public ConsoleWriter()
            {
                mLines = new List<string>();
                mCurrentLine = string.Empty;
            }

            public override Encoding Encoding => Encoding.UTF8;

            public override void Write(char value)
            {
                if (value == '\r')
                {
                    return;
                }

                if (value != '\n')
                {
                    mCurrentLine += value;
                }
                else
                {
                    Flush();
                }
            }

            public override void Flush()
            {
                mLines.Add(mCurrentLine);
                mCurrentLine = string.Empty;
            }

            public IReadOnlyList<string> Lines => mLines;

            private readonly List<string> mLines;
            private string mCurrentLine;
        }

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>The output of the specified command, in the form of lines.</returns>
        public static IReadOnlyList<string> Execute(string command)
        {
            string[] args = command.Tokenize();

            IConsoleCommand? currentCommand = null;
            int unusedArgsStart = -1;

            for (int i = 0; i < args.Length; i++)
            {
                IReadOnlyDictionary<string, IConsoleCommand> commandDict;
                if (currentCommand != null)
                {
                    commandDict = currentCommand.Subcommands;
                }
                else
                {
                    commandDict = sBaseCommands;
                }

                string argument = args[i];
                if (commandDict.ContainsKey(argument))
                {
                    currentCommand = commandDict[argument];
                }
                else
                {
                    unusedArgsStart = i;
                    break;
                }
            }

            if (currentCommand == null)
            {
                return new string[] { $"Command not recognized: {command}" };
            }
            else if (currentCommand.Execute == null)
            {
                return new string[] { $"Incomplete command: {command}" };
            }

            string[] commandArgs;
            if (unusedArgsStart < 0)
            {
                commandArgs = Array.Empty<string>();
            }
            else
            {
                commandArgs = args[unusedArgsStart..];
            }

            using var writer = new ConsoleWriter();
            currentCommand.Execute(commandArgs, writer);

            writer.Flush();
            return writer.Lines;
        }
    }
}
