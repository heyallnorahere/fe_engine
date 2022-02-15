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
using System.Diagnostics.CodeAnalysis;

namespace FEEngine.Test
{
    internal static class Utilities
    {
        public static Factory DefaultFactory
        {
            get
            {
                Factory? factory = Engine.GetFactory();
                return factory ?? throw new NullReferenceException("No default factory!");
            }
        }

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

            var factory = DefaultFactory;
            map = factory.Create<IMap>(mapDesc);
            unit = factory.Create<IUnit>(unitDesc);

            if (map != null && unit != null)
            {
                return map.AddUnit(unit) != -1;
            }

            return false;
        }
    }
}
