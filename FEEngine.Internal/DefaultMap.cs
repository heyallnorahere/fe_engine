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

namespace FEEngine.Internal
{
    internal class DefaultMap : IMap
    {
        public DefaultMap(MapDesc desc)
        {
            mDesc = desc;
            mUnits = new List<IUnit>();
            mActions = new List<Action>();
            ClientData = desc.ClientData;
        }

        public bool IsOutOfBounds(Vector point)
        {
            return point.X < 0 || point.Y < 0 || 
                point.X >= mDesc.Size.X || point.Y >= mDesc.Size.Y;
        }
        public Vector Size => mDesc.Size;
        public string? Name => mDesc.Name;

        public int AddUnit(IUnit unit)
        {
            int index = mUnits.IndexOf(unit);
            if (index != -1)
            {
                return index;
            }

            index = mUnits.FindIndex(u => u.Position == unit.Position);
            if (index != -1)
            {
                return -1;
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

        public int PushAction(Action action)
        {
            int index = mActions.Count;
            mActions.Add(action);
            return index;
        }

        public bool Flush()
        {
            var units = FindActionSubmitters();
            foreach (int index in units.Keys)
            {
                Action action = mActions[index];
                IUnit unit = units[index];

                if (!action.Invoke(unit))
                {
                    return false;
                }
            }

            mActions.Clear();
            foreach (IUnit unit in units.Values)
            {
                unit.ClearActions();
            }

            return true;
        }

        public IReadOnlyList<Tuple<IUnit, Action>> UndoActions(int count)
        {
            var unitActionPairs = new List<Tuple<IUnit, Action>>();

            var units = FindActionSubmitters();
            foreach (int index in units.Keys)
            {
                Action action = mActions[index];
                IUnit unit = units[index];

                unitActionPairs.Add(new Tuple<IUnit, Action>(unit, action));
            }
            
            var pairArray = unitActionPairs.ToArray();
            int startIndex = count > unitActionPairs.Count ? 0 : unitActionPairs.Count - count;
            var removedActions = pairArray[startIndex..];

            for (int i = 0; i < removedActions.Length; i++)
            {
                var pair = removedActions[i];
                pair.Item1.RemoveActionIndex(startIndex + i);
            }

            mActions.RemoveRange(startIndex, removedActions.Length);
            return removedActions;
        }

        private SortedDictionary<int, IUnit> FindActionSubmitters()
        {
            var units = new SortedDictionary<int, IUnit>();
            foreach (IUnit unit in mUnits)
            {
                foreach (int index in unit.ActionIndices)
                {
                    if (units.ContainsKey(index))
                    {
                        throw new ArgumentException("An index appeared more than once!");
                    }

                    units.Add(index, unit);
                }
            }

            return units;
        }

        public IReadOnlyList<IUnit> Units => mUnits;
        public IMapClientData? ClientData { get; set; }

        private readonly MapDesc mDesc;
        private readonly List<IUnit> mUnits;
        private readonly List<Action> mActions;
    }
}
