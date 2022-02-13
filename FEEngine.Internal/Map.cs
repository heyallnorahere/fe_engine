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

namespace FEEngine.Internal
{
    internal class Map : IMap
    {
        public Map(MapDesc desc)
        {
            mDesc = desc;
            mUnits = new List<IUnit>();
        }

        public bool IsOutOfBounds(Vector point) => point.X >= mDesc.Size.X || point.Y >= mDesc.Size.Y;
        public Vector Size => mDesc.Size;
        public string? Name => mDesc.Name;

        public int AddUnit(IUnit unit)
        {
            int index = mUnits.FindIndex(u => u == unit);
            if (index != -1)
            {
                return index;
            }

            if (unit.Map != null || IsOutOfBounds(unit.Position))
            {
                return -1;
            }

            index = mUnits.Count;
            mUnits.Add(unit);

            unit.SetMap(this);
            return index;
        }

        public IReadOnlyList<IUnit> Units => mUnits;

        private readonly MapDesc mDesc;
        private readonly List<IUnit> mUnits;
    }
}
