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

namespace FEEngine.Cmdline.Commands
{
    /// <summary>
    /// A helper class to help with creating subcommands;
    /// </summary>
    internal sealed class GenericSubcommand : IConsoleCommand
    {
        public GenericSubcommand(IReadOnlyDictionary<string, IConsoleCommand> subcommands)
        {
            Subcommands = subcommands;
            Execute = null;
        }

        public GenericSubcommand(ConsoleCommandExecutionCallback executionCallback)
        {
            Execute = executionCallback;
            Subcommands = new Dictionary<string, IConsoleCommand>();
        }

        public IReadOnlyDictionary<string, IConsoleCommand> Subcommands { get; }
        public ConsoleCommandExecutionCallback? Execute { get; }
    }
}
