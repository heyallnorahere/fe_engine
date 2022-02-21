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
    internal class DefaultEffectiveStats : EffectiveStats
    {
        public int Atk { get; set; } = -1;
        public int Hit { get; set; } = -1;
        public int Avo { get; set; } = 0;
        public int MagicAvo { get; set; } = 0;
        public int Prt { get; set; } = 0;
        public int Rsl { get; set; } = 0;
        public int AS { get; set; } = 0;
    }

    // https://fe16.triangleattack.com/
    internal class DefaultCombatEngine : ICombatEngine
    {
        public bool CanAttack(UnitCombatData userData, UnitCombatData targetData)
        {
            IUnit user = userData.Unit;
            IUnit target = targetData.Unit;

            IItem? weapon = user.EquippedWeapon;
            if (weapon == null || weapon.WeaponData == null)
            {
                return false;
            }

            WeaponData weaponData = weapon.WeaponData.Value;
            int minRange = weaponData.MinRange;
            int maxRange = weaponData.MaxRange;

            int distance = (target.Position - user.Position).TaxicabLength;
            return distance >= minRange && distance <= maxRange;
        }

        public IReadOnlyList<AttackData>? Calculate(UnitCombatData attackerData, UnitCombatData targetData)
        {
            if (!CanAttack(attackerData, targetData))
            {
                return null;
            }

            if (attackerData.EffectiveStats is DefaultEffectiveStats userStats &&
                targetData.EffectiveStats is DefaultEffectiveStats targetStats)
            {
                IUnit attacker = attackerData.Unit;
                IUnit target = attackerData.Unit;

                throw new NotImplementedException();
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

                int baseHit = weaponData.Hit; // todo: equipment, battalions, abilities
                if (magic)
                {
                    effectiveStats.Hit = baseHit + stats.Dexterity;
                }
                else
                {
                    effectiveStats.Hit = baseHit + ((stats.Dexterity + stats.Luck) / 2);
                }

                effectiveStats.Prt += stats.Defense;
                effectiveStats.Rsl += stats.Resistance;
                burden += weaponData.Weight;
            }

            // todo: equipment and battalions

            burden -= stats.Strength / 5;
            if (burden < 0)
            {
                burden = 0;
            }

            effectiveStats.AS = stats.Speed - burden;
            effectiveStats.Avo = effectiveStats.AS + baseAvo; // todo: terrain
            effectiveStats.MagicAvo = ((stats.Speed + stats.Luck) / 2) + baseAvo;

            return effectiveStats;
        }    
    }
}
