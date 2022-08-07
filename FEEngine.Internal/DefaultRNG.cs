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
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FEEngine.Internal
{
    internal class DefaultRNG : IRandomNumberGenerator
    {
        public const int Min = 0;
        public const int Max = 99;

        public DefaultRNG(RandomNumberGeneratorDesc desc)
        {
            mTrueHit = desc.TrueHit;
            if (desc.Seed != 0)
            {
                mGenerator = new Random(desc.Seed);
            }
            else
            {
                mGenerator = new Random();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool HitChance(int displayedPercentage) => ProcessCall(displayedPercentage);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool CritChance(int displayedPercentage) => ProcessCall(displayedPercentage);

        public TrueHitFlags TrueHit => mTrueHit;

        private bool ProcessCall(int displayedPercentage)
        {
            var stackframe = new StackFrame(1);
            var method = stackframe.GetMethod();

            string name = method!.Name;
            var flag = Enum.Parse<TrueHitFlags>(name);

            if (mTrueHit.HasFlag(flag))
            {
                int rn1 = Generate();
                int rn2 = Generate();

                int avg = (rn1 + rn2) / 2;
                return avg < displayedPercentage;
            }
            else
            {
                return Generate() < displayedPercentage;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int Generate() => mGenerator.Next(Min, Max);

        private readonly Random mGenerator;
        private readonly TrueHitFlags mTrueHit;
    }
}
