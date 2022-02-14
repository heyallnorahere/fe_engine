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
    internal class Unit : IUnit
    {
        public Unit(UnitDesc desc)
        {
            mName = desc.Name;
            mPosition = desc.StartingPosition;
            mMap = null;
            mActionIndices = new List<int>();
        }

        public string? Name => mName;
        public Vector Position
        {
            get => mPosition;
            set => mPosition = value;
        }

        public IMap? Map => mMap;
        public void SetMap(IMap map) => mMap = map;

        public bool AddAction(Action action)
        {
            if (mMap == null)
            {
                return false;
            }

            int index = mMap.PushAction(action);
            mActionIndices.Add(index);
            return true;
        }

        public void ClearActions() => mActionIndices.Clear();
        public IReadOnlyList<int> ActionIndices => mActionIndices;

        private readonly string? mName;
        private readonly List<int> mActionIndices;
        private Vector mPosition;
        private IMap? mMap;
    }
}
