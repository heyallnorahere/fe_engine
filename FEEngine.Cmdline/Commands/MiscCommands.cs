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

using System.Collections.Generic;
using System.IO;

namespace FEEngine.Cmdline.Commands
{
    [RegisteredCommand("clear")]
    public sealed class ClearCommand : IConsoleCommand
    {
        public ClearCommand()
        {
            Subcommands = new Dictionary<string, IConsoleCommand>();
        }

        public IReadOnlyDictionary<string, IConsoleCommand> Subcommands { get; }
        public ConsoleCommandExecutionCallback? Execute => ExecuteImpl;

        private static void ExecuteImpl(string[] args, Stream output)
        {
            var writer = output.CreateWriter();
            if (args.Length > 0)
            {
                writer.WriteLine("Usage: clear");
                return;
            }

            Renderer.Clear();
            writer.WriteLine("Cleared the screen for redrawing!");
        }
    }

    [RegisteredCommand("quit")]
    public sealed class QuitCommand : IConsoleCommand
    {
        public QuitCommand()
        {
            Subcommands = new Dictionary<string, IConsoleCommand>();
        }

        public IReadOnlyDictionary<string, IConsoleCommand> Subcommands { get; }
        public ConsoleCommandExecutionCallback? Execute => ExecuteImpl;

        private static void ExecuteImpl(string[] args, Stream output)
        {
            var writer = output.CreateWriter();
            if (args.Length > 0)
            {
                writer.WriteLine("Usage: quit");
                return;
            }

            Renderer.SignalCtrlC();
        }
    }
}
