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
using System;
using System.Collections.Generic;

namespace FEEngine.Cmdline
{
    public enum Allegiance
    {
        Player,
        Enemy
    }

    /// <summary>
    /// A phase manager keeps track of whose turn it is to move.
    /// </summary>
    public sealed class PhaseManager
    {
        private static readonly List<Allegiance> sAllegiances;
        static PhaseManager()
        {
            var allegiances = Enum.GetValues<Allegiance>();
            sAllegiances = new List<Allegiance>(allegiances);
        }

        public PhaseManager()
        {
            CurrentPhase = Allegiance.Player;
            mMap = null;
        }

        /// <summary>
        /// Ends the current phase and advances.
        /// </summary>
        public void EndPhase()
        {
            int currentIndex = sAllegiances.IndexOf(CurrentPhase);
            if (currentIndex < 0)
            {
                throw new ArgumentException("Invalid current phase!");
            }

            if (mMap != null)
            {
                if (!mMap.Flush())
                {
                    throw new Exception("A map action has failed!");
                }

                foreach (IUnit unit in mMap.Units)
                {
                    if (unit.ClientData is UnitClientData data)
                    {
                        if (data.Allegiance != CurrentPhase)
                        {
                            continue;
                        }

                        data.HasMoved = false;
                    }
                    else
                    {
                        throw new Exception("Invalid unit!");
                    }
                }
            }

            int nextIndex = (currentIndex + 1) % sAllegiances.Count;
            CurrentPhase = sAllegiances[nextIndex];
        }

        internal void SetMap(IMap map) => mMap = map;

        /// <summary>
        /// The current phase in play.
        /// </summary>
        public Allegiance CurrentPhase { get; set; }

        private IMap? mMap;
    }
}
