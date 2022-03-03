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

namespace FEEngine.Internal
{
    internal class DefaultEffectiveStats : EffectiveStats
    {
        public int Atk { get; set; } = 0;
        public int Hit { get; set; } = 0;
        public int Crit { get; set; } = 0;
        public int Avo { get; set; } = 0;
        public int MagicAvo { get; set; } = 0;
        public int CritAvo { get; set; } = 0;
        public int Prt { get; set; } = 0;
        public int Rsl { get; set; } = 0;
        public int AS { get; set; } = 0;
    }

    /// <summary>
    /// Formulas from https://fe16.triangleattack.com/.
    /// </summary>
    internal class DefaultCombatEngine : ICombatEngine
    {
        private class InternalWeaponData
        {
            public IItem? Item { get; set; }
            public WeaponData Data { get; set; }
        }

        private static bool GetWeaponData(IUnit unit, [NotNullWhen(true)] out InternalWeaponData? data)
        {
            IItem? item = unit.EquippedWeapon;
            if (item != null)
            {
                WeaponData? weaponData = item.WeaponData;
                if (weaponData != null)
                {
                    data = new InternalWeaponData
                    {
                        Item = item,
                        Data = weaponData.Value
                    };

                    return true;
                }
            }

            data = null;
            return false;
        }

        public bool CanAttack(UnitCombatData userData, UnitCombatData targetData)
        {
            IUnit user = userData.Unit;
            IUnit target = targetData.Unit;

            if (!GetWeaponData(user, out InternalWeaponData? weaponData))
            {
                return false;
            }

            int minRange = weaponData.Data.MinRange;
            int maxRange = weaponData.Data.MaxRange;

            int distance = (target.Position - user.Position).TaxicabLength;
            return distance >= minRange && distance <= maxRange;
        }

        private struct InternalUnitCombatData
        {
            public IUnit Unit;
            public DefaultEffectiveStats Stats;
            public InternalWeaponData? Weapon;
        }

        private static AttackData Calculate(InternalUnitCombatData attackerData, InternalUnitCombatData targetData, bool counter)
        {
            if (attackerData.Weapon == null)
            {
                throw new ArgumentException("Cannot attack without a weapon!");
            }

            bool magic = attackerData.Weapon.Data.Type == WeaponType.Magic;
            int defense = magic ? targetData.Stats.Rsl : targetData.Stats.Prt;
            int avo = magic ? targetData.Stats.MagicAvo : targetData.Stats.Avo;

            return new AttackData
            {
                Damage = attackerData.Stats.Atk - defense,
                Hit = attackerData.Stats.Hit - avo,
                Crit = attackerData.Stats.Crit - targetData.Stats.CritAvo,

                Counter = counter
            };
        }

        public RoundData? Calculate(UnitCombatData attackerData, UnitCombatData targetData)
        {
            if (!CanAttack(attackerData, targetData))
            {
                return null;
            }

            if (attackerData.EffectiveStats is DefaultEffectiveStats attackerStats &&
                targetData.EffectiveStats is DefaultEffectiveStats targetStats)
            {
                var attackData = new List<AttackData>();
                var indices = new List<int>();

                IUnit attacker = attackerData.Unit;
                IUnit target = attackerData.Unit;

                int attackerStrikeIndex = -1;
                int targetStrikeIndex = -1;

                var internalAttackerData = new InternalUnitCombatData
                {
                    Unit = attacker,
                    Stats = attackerStats
                };

                var internalTargetData = new InternalUnitCombatData
                {
                    Unit = target,
                    Stats = targetStats
                };

                GetWeaponData(attacker, out internalAttackerData.Weapon);
                GetWeaponData(target, out internalTargetData.Weapon);

                var addAttack = (bool counter) =>
                {
                    int index;
                    InternalUnitCombatData attacking, defending;

                    if (counter)
                    {
                        index = targetStrikeIndex;
                        attacking = internalTargetData;
                        defending = internalAttackerData;
                    }
                    else
                    {
                        index = attackerStrikeIndex;
                        attacking = internalAttackerData;
                        defending = internalTargetData;
                    }

                    if (index < 0)
                    {
                        index = attackData.Count;
                        if (counter)
                        {
                            targetStrikeIndex = index;
                        }
                        else
                        {
                            attackerStrikeIndex = index;
                        }

                        var data = Calculate(attacking, defending, counter);
                        attackData.Add(data);
                    }

                    indices.Add(index);
                };

                // todo: check for vantage or something
                addAttack(false);

                bool canCounter = CanAttack(targetData, attackerData);
                if (canCounter)
                {
                    addAttack(true);
                }

                // todo: check for brave weapons

                int asDifference = attackerStats.AS - targetStats.AS;
                if (asDifference >= 4)
                {
                    addAttack(false);
                }
                else if (asDifference <= -4 && canCounter)
                {
                    addAttack(true);
                }

                return new RoundData
                {
                    Attacker = attackerData,
                    Target = targetData,
                    Data = attackData,
                    Indices = indices
                };
            }

            return null;
        }

        public CombatResult? Execute(AttackData data)
        {
            throw new NotImplementedException();
        }

        public UnitCombatData GetCombatData(IUnit unit)
        {
            return new UnitCombatData
            {
                EffectiveStats = CalculateEffectiveStats(unit),
                Unit = unit
            };
        }

        private static EffectiveStats CalculateEffectiveStats(IUnit unit)
        {
            var stats = unit.Stats;
            var effectiveStats = new DefaultEffectiveStats();

            int burden = 0;
            int baseAvo = 0;

            IItem? weapon = unit.EquippedWeapon;
            if (weapon != null)
            {
                WeaponData weaponData = weapon.WeaponData ?? throw new ArgumentException("The equipped weapon is not a weapon!");

                bool magic = weaponData.Type == WeaponType.Magic;
                effectiveStats.Atk = weaponData.Might + (magic ? stats.Magic : stats.Strength);
                effectiveStats.Crit = weaponData.Crit + (stats.Dexterity + stats.Luck) / 2;

                if (magic)
                {
                    effectiveStats.Hit = weaponData.Hit + stats.Dexterity;
                }
                else
                {
                    effectiveStats.Hit = weaponData.Hit + ((stats.Dexterity + stats.Luck) / 2);
                }

                // todo: equipment, battalions, abilities

                effectiveStats.Prt += stats.Defense;
                effectiveStats.Rsl += stats.Resistance;
                burden += weaponData.Weight;
            }

            // todo: equipment, battalions, combat art

            burden -= stats.Strength / 5;
            if (burden < 0)
            {
                burden = 0;
            }

            effectiveStats.AS = stats.Speed - burden;
            effectiveStats.Avo = effectiveStats.AS + baseAvo; // todo: terrain
            effectiveStats.MagicAvo = ((stats.Speed + stats.Luck) / 2) + baseAvo;
            effectiveStats.CritAvo = stats.Luck;

            Type effectiveStatsType = effectiveStats.GetType();
            var properties = effectiveStatsType.GetProperties();
            foreach (var property in properties)
            {
                object? value = property.GetValue(effectiveStats);
                if (value != null && (int)value < 0)
                {
                    property.SetValue(effectiveStats, 0);
                }
            }

            return effectiveStats;
        }    
    }
}
