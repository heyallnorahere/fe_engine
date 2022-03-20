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

using FEEngine.Cmdline.ClientData;
using FEEngine.Cmdline.UI.Views;
using System;
using System.Collections.Generic;
using System.IO;

namespace FEEngine.Cmdline.Commands
{
    [RegisteredCommand("unit")]
    public sealed class UnitCommand : IConsoleCommand
    {
        public UnitCommand()
        {
            Subcommands = new Dictionary<string, IConsoleCommand>
            {
                ["get"] = new GenericSubcommand
                {
                    Execute = Get
                }
            };
        }

        public IReadOnlyDictionary<string, IConsoleCommand> Subcommands { get; }
        public ConsoleCommandExecutionCallback? Execute => null;

        private static void Get(string[] args, Stream output)
        {
            const string usage = 
                "Usage: unit get <x: int> <y: int> OR\n" +
                "       unit get selected OR\n" +
                "       unit get hovered";

            var writer = output.CreateWriter();
            if (args.Length > 2 || args.Length == 0)
            {
                writer.WriteLine(usage);
                return;
            }

            IUnit? unitAt(Vector position)
            {
                var map = Program.Instance.Map;
                IUnit? found = map.UnitAt(position);
                if (found == null)
                {
                    writer?.WriteLine($"No unit found at: ({position.X}, {position.Y})");
                }

                return found;
            };

            IUnit unit;
            if (args.Length > 1)
            {
                Vector position;
                if (!int.TryParse(args[0], out position.X) ||
                    !int.TryParse(args[1], out position.Y))
                {
                    writer.WriteLine(usage);
                    return;
                }

                var found = unitAt(position);
                if (found != null)
                {
                    unit = found;
                }
                else
                {
                    return;
                }
            }
            else
            {
                var mapView = GetMapView();
                switch (args[0])
                {
                    case "hovered":
                        var found = unitAt(mapView.CursorPos);
                        if (found != null)
                        {
                            unit = found;
                            break;
                        }
                        else
                        {
                            return;
                        }
                    case "selected":
                        if (mapView.SelectedUnit != null)
                        {
                            unit = mapView.SelectedUnit;
                            break;
                        }
                        else
                        {
                            writer.WriteLine("No unit is selected!");
                            return;
                        }
                    default:
                        writer.WriteLine(usage);
                        return;
                }
            }

            var unitPosition = unit.Position;
            writer.WriteLine($"Unit at ({unitPosition.X}, {unitPosition.Y}):");

            var stats = unit.Stats;
            writer.WriteLine($"  Name: {unit.Name ?? "null"}");
            writer.WriteLine($"  HP: {unit.HP}/{stats.HP}");
            writer.WriteLine($"  Level: {stats.Level}");
            writer.WriteLine($"  Movement: {stats.Movement}");

            writer.WriteLine("  Stats:\n" +
                $"    Strength: {stats.Strength}\n" +
                $"    Magic: {stats.Magic}\n" +
                $"    Speed: {stats.Speed}\n" +
                $"    Dexterity: {stats.Dexterity}\n" +
                $"    Defense: {stats.Defense}\n" +
                $"    Resistance: {stats.Resistance}\n" +
                $"    Luck: {stats.Luck}\n" +
                $"    Charm: {stats.Charm}");
        }

        private static MapView GetMapView()
        {
            var uiRoot = Program.Instance.UIRoot;
            var view = uiRoot.FindView<MapView>();
            return view ?? throw new Exception("Could not get map view!");
        }
    }
}
