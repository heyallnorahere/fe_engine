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
        private class PhaseCommand : IConsoleCommand
        {
            public PhaseCommand()
            {
                Subcommands = new Dictionary<string, IConsoleCommand>
                {
                    ["get"] = new GenericSubcommand
                    {
                        Execute = Get
                    },
                    ["end"] = new GenericSubcommand
                    {
                        Execute = End
                    }
                };
            }

            public IReadOnlyDictionary<string, IConsoleCommand> Subcommands { get; }
            public ConsoleCommandExecutionCallback? Execute => null;

            private static void Get(string[] args, Stream output)
            {
                var writer = output.CreateWriter();
                if (args.Length > 0)
                {
                    writer.WriteLine("Usage: map phase get");
                    return;
                }

                var map = Program.Instance.Map;
                var phaseManager = map.GetPhaseManager();
                if (phaseManager == null)
                {
                    writer.WriteLine("Could not get the current map's phase manager!");
                    return;
                }

                var currentPhase = phaseManager.CurrentPhase;
                writer.WriteLine($"Current phase: {currentPhase}");
            }

            private static void End(string[] args, Stream output)
            {
                var writer = output.CreateWriter();
                if (args.Length > 0)
                {
                    writer.WriteLine("Usage: map phase end");
                    return;
                }

                var map = Program.Instance.Map;
                var phaseManager = map.GetPhaseManager();
                if (phaseManager == null)
                {
                    writer.WriteLine("Could not get the current map's phase manager!");
                    return;
                }

                try
                {
                    phaseManager.EndPhase();
                    var currentPhase = phaseManager.CurrentPhase;
                    writer.WriteLine($"Advanced to phase: {currentPhase}");
                }
                catch (Exception ex)
                {
                    Type type = ex.GetType();
                    writer.WriteLine($"{type.Name} caught: {ex.Message}");
                }
            }
        }

        public MapCommand()
        {
            Subcommands = new Dictionary<string, IConsoleCommand>
            {
                ["phase"] = new PhaseCommand(),
                ["load"] = new GenericSubcommand
                {
                    Execute = Load
                },
                ["flush"] = new GenericSubcommand
                {
                    Execute = Flush
                }
            };
        }

        public IReadOnlyDictionary<string, IConsoleCommand> Subcommands { get; }
        public ConsoleCommandExecutionCallback? Execute => null;

        private static void Load(string[] args, Stream output)
        {
            var writer = output.CreateWriter();
            if (args.Length != 1)
            {
                writer.WriteLine("Usage: map load <descriptor: string>");
                return;
            }

            try
            {
                string mapDescriptor = args[0];
                Program.Instance.LoadMap(mapDescriptor);
                writer.WriteLine($"Successfully loaded map: {mapDescriptor}");
            }
            catch (Exception ex)
            {
                Type type = ex.GetType();
                writer.WriteLine($"{type.Name} caught: {ex.Message}");
            }
        }

        private static void Flush(string[] args, Stream output)
        {
            var writer = output.CreateWriter();
            if (args.Length > 0)
            {
                writer.WriteLine("Usage: map flush");
                return;
            }

            var map = Program.Instance.Map;
            if (!map.Flush())
            {
                writer.WriteLine("One or more actions failed!");
            }
            else
            {
                writer.WriteLine("Successfully flushed the map!");
            }
        }
    }
}
