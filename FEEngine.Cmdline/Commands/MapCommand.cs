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

namespace FEEngine.Cmdline.Commands
{
    [RegisteredCommand("map")]
    public sealed class MapCommand : IConsoleCommand
    {
        public MapCommand()
        {
            Subcommands = new Dictionary<string, IConsoleCommand>
            {
                ["load"] = new GenericSubcommand
                {
                    Execute = Load
                }
            };
        }

        public IReadOnlyDictionary<string, IConsoleCommand> Subcommands { get; }
        public ConsoleCommandExecutionCallback? Execute => null;

        private static void Load(string[] args, Stream output)
        {
            var writer = new StreamWriter(output)
            {
                AutoFlush = true
            };

            if (args.Length != 1)
            {
                writer.WriteLine("Usage: map load <descriptor: string>");
                return;
            }

            string mapDescriptor = args[0];
            try
            {
                Program.Instance.LoadMap(mapDescriptor);
                writer.WriteLine($"Successfully loaded map: {mapDescriptor}");
            }
            catch (Exception ex)
            {
                Type type = ex.GetType();
                writer.WriteLine($"{type.FullName ?? type.Name} caught: {ex.Message}");
            }
        }
    }
}
