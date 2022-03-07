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
using Xunit;

namespace FEEngine.Test
{
    public class CombatTests
    {
        private class TestRNG : IRandomNumberGenerator
        {
            public TestRNG(bool crit)
            {
                mCrit = crit;
            }

            public bool HitChance(int displayedPercentage) => true;
            public bool CritChance(int displayedPercentage) => mCrit;

            private readonly bool mCrit;
        }

        [Theory]
        [InlineData(5)]
        [InlineData(60)]
        [InlineData(43)]
        [InlineData(92)]
        [InlineData(11)]
        public void EffectiveStats(int seed)
        {
            var random = new Random(seed);
            object statObj = new UnitStats
            {
                Movement = 5,
                Level = 1
            };

            var statsType = typeof(UnitStats);
            var fields = statsType.GetFields();

            foreach (var field in fields)
            {
                int value = random.Next(1, 100);
                field.SetValue(statObj, value);
            }

            var stats = (UnitStats)statObj;
            var prototype = Utilities.ItemPrototypes["iron-sword"];
            var unitDesc = new UnitDesc
            {
                Name = "Test Unit",
                EquippedWeapon = prototype.Instantiate(),
                Stats = stats
            };

            var combatEngineDesc = new CombatEngineDesc
            {
                RNG = new TestRNG(false)
            };

            Factory? factory = Engine.GetFactory();
            IUnit? unit = factory!.Create<IUnit>(unitDesc);
            ICombatEngine? combatEngine = factory!.Create<ICombatEngine>(combatEngineDesc);

            Assert.NotNull(unit?.EquippedWeapon);
            Assert.NotNull(combatEngine);

            var effectiveStatsObj = combatEngine.GetCombatData(unit!).EffectiveStats;
            var effectiveStatValues = effectiveStatsObj.ToDictionary();

            // todo: equipment, battalions, abilities, etc.
            WeaponData weaponData = (WeaponData)unit?.EquippedWeapon?.WeaponData!;

            int validAtk, validHit;
            int validCrit = (stats.Dexterity + stats.Luck) / 2 + weaponData.Crit;

            if (weaponData.Type == WeaponType.Magic)
            {
                validAtk = stats.Magic + weaponData.Might;
                validHit = (stats.Dexterity + stats.Luck) / 2 + weaponData.Hit;
            }
            else
            {
                validAtk = stats.Strength + weaponData.Might;
                validHit = stats.Dexterity + weaponData.Hit;
            }

            Assert.Equal(validAtk, effectiveStatValues["Atk"]);
            Assert.Equal(validHit, effectiveStatValues["Hit"]);
            Assert.Equal(validCrit, effectiveStatValues["Crit"]);

            int burden = weaponData.Weight - stats.Strength / 5;
            if (burden < 0)
            {
                burden = 0;
            }

            int validAS = stats.Speed - burden;
            int baseAvo = 0; // todo: equipment, battalions, abilities, etc.
            int validAvo = validAS + baseAvo; // todo: terrain
            int validMagicAvo = (stats.Speed + stats.Luck) / 2 + baseAvo;
            int validCritAvo = stats.Luck; // todo: combat art

            Assert.Equal(validAS, effectiveStatValues["AS"]);
            Assert.Equal(validAvo, effectiveStatValues["Avo"]);
            Assert.Equal(validMagicAvo, effectiveStatValues["MagicAvo"]);
            Assert.Equal(validCritAvo, effectiveStatValues["CritAvo"]);

            // todo: battalion, equipment
            int validPrt = stats.Defense;
            int validRsl = stats.Resistance;

            Assert.Equal(validPrt, effectiveStatValues["Prt"]);
            Assert.Equal(validRsl, effectiveStatValues["Rsl"]);
        }
    }
}
