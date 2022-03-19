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

using FEEngine.Cmdline.UI.Views;
using System.Collections.Generic;
using System.IO;

namespace FEEngine.Cmdline.Commands
{
    [RegisteredCommand("cursor")]
    public sealed class CursorCommand : IConsoleCommand
    {
        public CursorCommand()
        {
            Subcommands = new Dictionary<string, IConsoleCommand>
            {
                ["move"] = new GenericSubcommand
                {
                    Execute = Move
                },
                ["get"] = new GenericSubcommand
                {
                    Execute = Get
                }
            };
        }

        public IReadOnlyDictionary<string, IConsoleCommand> Subcommands { get; }
        public ConsoleCommandExecutionCallback? Execute => null;

        private static void Move(string[] args, Stream output)
        {
            Vector newPosition;
            var writer = output.CreateWriter();

            if (args.Length != 2 ||
                !int.TryParse(args[0], out newPosition.X) ||
                !int.TryParse(args[1], out newPosition.Y))
            {
                writer.WriteLine("Usage: cursor move <x: int> <y: int>");
                return;
            }

            var map = Program.Instance.Map;
            if (map.IsOutOfBounds(newPosition))
            {
                writer.WriteLine($"Invalid cursor position: ({newPosition.X}, {newPosition.Y})");
                return;
            }

            var uiRoot = Program.Instance.UIRoot;
            var mapView = uiRoot.FindView<MapView>();

            if (mapView == null)
            {
                writer.WriteLine("Could not find the map view!");
                return;
            }

            mapView.CursorPos = newPosition;
            writer.WriteLine($"Cursor moved to: ({newPosition.X}, {newPosition.Y})");
        }

        private static void Get(string[] args, Stream output)
        {
            var writer = output.CreateWriter();
            if (args.Length > 0)
            {
                writer.WriteLine("Usage: cursor get");
                return;
            }

            var uiRoot = Program.Instance.UIRoot;
            var mapView = uiRoot.FindView<MapView>();

            if (mapView == null)
            {
                writer.WriteLine("Could not find the map view!");
                return;
            }

            var cursorPos = mapView.CursorPos;
            writer.WriteLine($"The cursor is currently at: ({cursorPos.X}, {cursorPos.Y})");
        }
    }
}
