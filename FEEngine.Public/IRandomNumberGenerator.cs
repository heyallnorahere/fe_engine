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

namespace FEEngine
{
    [Flags]
    public enum TrueHitFlags
    {
        HitChance,
        CritChance
    }

    public struct RandomNumberGeneratorDesc : ICreationDesc
    {
        public RandomNumberGeneratorDesc()
        {
            Seed = 0;
            TrueHit = TrueHitFlags.HitChance;
        }

        public int Seed;
        public TrueHitFlags TrueHit;

        public bool Verify() => true;
        public ICreationDesc Clone() => (ICreationDesc)MemberwiseClone();
    }

    /// <summary>
    /// Evaluates if an event is triggered based on a percentage chance.
    /// </summary>
    [FactoryInterface]
    public interface IRandomNumberGenerator
    {
        /// <summary>
        /// Determines if an attack will hit. Should implement True Hit.
        /// </summary>
        /// <param name="displayedPercentage">The displayed hit chance.</param>
        /// <returns>If the attack hit.</returns>
        public bool HitChance(int displayedPercentage);

        /// <summary>
        /// Determines if a critical hit is triggered.
        /// </summary>
        /// <param name="displayedPercentage">The displayed crit chance.</param>
        /// <returns>If the attack was a crit.</returns>
        public bool CritChance(int displayedPercentage);

        /// <summary>
        /// Describes the calculations in which this generator uses True Hit.
        /// </summary>
        public TrueHitFlags TrueHit { get; }
    }
}
