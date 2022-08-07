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
using System.Diagnostics.CodeAnalysis;

namespace FEEngine.Test
{
    internal static class Utilities
    {
        static Utilities()
        {
            Factory? factory = Engine.GetFactory();
            if (factory == null)
            {
                throw new Exception("No default factory exists!");
            }

            var assembly = typeof(Utilities).Assembly;
            ItemPrototypes = ManifestItemParser.Load(assembly, factory, new ManifestItemParser.Callbacks
            {
                KeyParser = KeyParser,
                ResourceQualifier = ResourceQualifier
            });
        }

        private static bool ResourceQualifier(string key) => key.StartsWith("FEEngine.Test.ItemPrototypes.");
        private static void KeyParser(ref string key)
        {
            int lastSeparator = key.LastIndexOf('.');
            if (lastSeparator < 0)
            {
                throw new ArgumentException("Invalid resource name!");
            }

            int filenameSeparator = key.LastIndexOf('.', lastSeparator - 1);
            if (filenameSeparator < 0)
            {
                throw new ArgumentException("Invalid resource name!");
            }

            int filenameStart = filenameSeparator + 1;
            key = key[filenameStart..lastSeparator];
        }

        public static IReadOnlyDictionary<string, FactoryPrototype<IItem>> ItemPrototypes { get; }

        public static bool SetupTestMap(Vector size, Vector pos,
            [NotNullWhen(true)] out IMap? map, [NotNullWhen(true)] out IUnit? unit)
        {
            var mapDesc = new MapDesc
            {
                Size = size,
                Name = "Test Map"
            };

            var unitDesc = new UnitDesc
            {
                Name = "Test Unit",
                StartingPosition = pos
            };

            var factory = Engine.GetFactory();
            map = factory?.Create<IMap>(mapDesc);
            unit = factory?.Create<IUnit>(unitDesc);

            if (map != null && unit != null)
            {
                return map.AddUnit(unit) != -1;
            }

            return false;
        }
    }
}
