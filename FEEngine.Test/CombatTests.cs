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
using System.Diagnostics.CodeAnalysis;
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
            var prototype = Utilities.ItemPrototypes["GenericSword"];
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

        private static bool SetupCombatEnvironment(Factory factory, [NotNullWhen(true)] out IMap? map, [NotNullWhen(true)] out ICombatEngine? combatEngine)
        {
            var combatEngineDesc = new CombatEngineDesc
            {
                RNG = new TestRNG(false)
            };

            combatEngine = factory.Create<ICombatEngine>(combatEngineDesc);
            if (combatEngine == null)
            {
                map = null;
                return false;
            }

            var mapDesc = new MapDesc
            {
                Size = (2, 1),
                Name = "Test Map"
            };

            var stats = new UnitStats
            {
                Movement = 5,
                Level = 1,
                HP = 20,
                Strength = 10,
                Magic = 0,
                Speed = 7,
                Dexterity = 12,
                Defense = 8,
                Resistance = 6,
                Luck = 2,
                Charm = 0
            };

            map = factory.Create<IMap>(mapDesc);
            if (map == null)
            {
                return false;
            }

            var prototype = Utilities.ItemPrototypes["GenericSword"];
            for (int i = 0; i < map.Size.X; i++)
            {
                var unitDesc = new UnitDesc
                {
                    Name = $"Soldier {i + 1}",
                    EquippedWeapon = prototype.Instantiate(),
                    StartingPosition = (i, 0),
                    Stats = stats
                };

                IUnit? unit = factory.Create<IUnit>(unitDesc);
                if (unit == null)
                {
                    return false;
                }

                int index = map.AddUnit(unit);
                if (index < 0)
                {
                    return false;
                }
            }

            return true;
        }

        [Fact]
        public void Combat()
        {
            var factory = Engine.GetFactory();
            bool succeeded = SetupCombatEnvironment(factory!, out IMap? map, out ICombatEngine? combatEngine);
            Assert.True(succeeded);

            IUnit attacker = map!.Units[0];
            IUnit target = map.Units[1];

            var attackerData = combatEngine!.GetCombatData(attacker);
            var targetData = combatEngine.GetCombatData(target);

            var roundData = combatEngine.Calculate(attackerData, targetData);
            Assert.NotNull(roundData);

            Assert.Equal(combatEngine, roundData.Value.Engine);
            Assert.Equal(attackerData, roundData.Value.Attacker);
            Assert.Equal(targetData, roundData.Value.Target);

            Assert.Equal(2, roundData.Value.Indices.Count);
            Assert.Equal(2, roundData.Value.Data.Count);

            foreach (int index in roundData.Value.Indices)
            {
                UnitCombatData currentAttacker, currentTarget;
                var data = roundData.Value.Data[index];

                if (data.Counter)
                {
                    currentAttacker = targetData;
                    currentTarget = attackerData;
                }
                else
                {
                    currentAttacker = attackerData;
                    currentTarget = targetData;
                }

                var result = combatEngine.Execute(data, currentAttacker, currentTarget);
                Assert.NotNull(result);

                Assert.True(result!.Value.DidHit);
                Assert.False(result.Value.DidCrit);
                Assert.False(result.Value.DidKill);
            }
        }

        [Fact]
        public void AttackAction()
        {
            var factory = Engine.GetFactory();
            bool succeeded = SetupCombatEnvironment(factory!, out IMap? map, out ICombatEngine? combatEngine);
            Assert.True(succeeded);

            IUnit attacker = map!.Units[0];
            IUnit target = map.Units[1];

            var attackerData = combatEngine!.GetCombatData(attacker);
            var targetData = combatEngine.GetCombatData(target);

            var roundData = combatEngine.Calculate(attackerData, targetData);
            Assert.NotNull(roundData);

            Assert.Equal(combatEngine, roundData.Value.Engine);
            Assert.Equal(attackerData, roundData.Value.Attacker);
            Assert.Equal(targetData, roundData.Value.Target);

            Assert.Equal(2, roundData.Value.Indices.Count);
            Assert.Equal(2, roundData.Value.Data.Count);

            var results = new List<CombatResult?>();
            var actionArgs = new AttackActionArgs
            {
                RoundData = roundData,
                Results = results
            };

            var action = Action.Create(Action.ID.Attack, actionArgs);
            Assert.NotNull(action);
            attacker.AddAction(action);

            succeeded = map.Flush();
            Assert.True(succeeded);

            Assert.Equal(2, results.Count);
            foreach (var result in results)
            {
                Assert.NotNull(result);
                Assert.True(result.Value.DidHit);
                Assert.False(result.Value.DidCrit);
                Assert.False(result.Value.DidKill);
            }
        }
    }
}
