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

namespace FEEngine.Internal
{
    internal class DefaultRNG : IRandomNumberGenerator
    {
        public DefaultRNG(RandomNumberGeneratorDesc desc)
        {
            if (desc.Seed >= 0)
            {
                mRandom = new Random(desc.Seed);
            }
            else
            {
                mRandom = new Random();
            }
        }

        public bool HitChance(int displayedPercentage)
        {
            // todo: true hit
            return true;
        }

        public bool CritChance(int displayedPercentage)
        {
            int result = mRandom.Next(0, 100);
            return result <= displayedPercentage;
        }

        private readonly Random mRandom;
    }
}
