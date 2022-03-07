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
using System.Reflection;

namespace FEEngine
{
    public struct CombatEngineDesc : ICreationDesc
    {
        public CombatEngineDesc()
        {
            RNG = null;
        }

        public IRandomNumberGenerator? RNG;

        public bool Verify() => true;
        public ICreationDesc Clone() => (ICreationDesc)MemberwiseClone();
    }

    public abstract class EffectiveStats
    {
        public IReadOnlyDictionary<string, int> ToDictionary()
        {
            Type type = GetType();
            var properties = type.GetProperties();

            var dictionary = new Dictionary<string, int>();
            foreach (var property in properties)
            {
                MethodInfo? getMethod = property.GetMethod;
                if (getMethod == null)
                {
                    continue;
                }

                if (getMethod.GetParameters().Length != 0)
                {
                    continue;
                }

                if (property.PropertyType != typeof(int))
                {
                    continue;
                }

                object? data = property.GetValue(this);
                if (data is int value)
                {
                    dictionary.Add(property.Name, value);
                }
            }

            return dictionary;
        }
    }

    public struct UnitCombatData
    {
        /// <summary>
        /// Effective stats, such as Atk and AS. Dependent on <see cref="IUnit.Stats"/>.
        /// </summary>
        public EffectiveStats EffectiveStats;

        /// <summary>
        /// The unit itself.
        /// </summary>
        public IUnit Unit;
    }

    public struct AttackData
    {
        /// <summary>
        /// The damage to deal to the target
        /// </summary>
        public int Damage;

        /// <summary>
        /// The percentage chance of the attack hitting.
        /// </summary>
        public int Hit;

        /// <summary>
        /// The percentage chance of a critical hit (x3) occuring.
        /// </summary>
        public int Crit;

        /// <summary>
        /// If this attack is a counter.
        /// </summary>
        public bool Counter;
    }

    public struct RoundData
    {
        /// <summary>
        /// The data describing the attacking unit.
        /// </summary>
        public UnitCombatData Attacker;

        /// <summary>
        /// The data describing the defending unit.
        /// </summary>
        public UnitCombatData Target;

        /// <summary>
        /// A list of objects describing strikes.
        /// Is not iterated on by the interpreter.
        /// See <see cref="Indices"/>.
        /// </summary>
        public IReadOnlyList<AttackData> Data;

        /// <summary>
        /// Indices of elements in <see cref="Data"/> to be executed by the interpreter.
        /// </summary>
        public IReadOnlyList<int> Indices;
    }

    public struct CombatResult
    {
        /// <summary>
        /// How much damage was dealt to the target.
        /// </summary>
        public int DamageDealt;

        /// <summary>
        /// If the attack hit.
        /// </summary>
        public bool DidHit;

        /// <summary>
        /// If the attack hit a critical hit (3x damage).
        /// </summary>
        public bool DidCrit;
    }

    /// <summary>
    /// A combat engine carries out combat instructions.
    /// </summary>
    [FactoryInterface]
    public interface ICombatEngine
    {
        /// <summary>
        /// Checks if one unit can attack another.
        /// The user must:
        /// <list type="bullet">
        /// <item>Have a weapon equipped.</item>
        /// <item>Be in attacking range of the target.</item>
        /// </list>
        /// </summary>
        /// <param name="attackerData">The data describing the attacking unit.</param>
        /// <param name="targetData">The data describing the unit to attack.</param>
        /// <returns>If the attacking unit can actually attack.</returns>
        public bool CanAttack(UnitCombatData attackerData, UnitCombatData targetData);

        /// <summary>
        /// Calculates a combat round between two units, given their current circumstances.
        /// If the target unit cannot counterattack, only calculates data for the initiating unit's attacks.
        /// </summary>
        /// <param name="attackerData">The data describing the unit initiating.</param>
        /// <param name="targetData">The data describing the unit counterattacking.</param>
        /// <returns>
        /// If the initiating unit can attack the target, returns the calculated data.
        /// Otherwise, returns null.
        /// </returns>
        public RoundData? Calculate(UnitCombatData attackerData, UnitCombatData targetData);

        /// <summary>
        /// Executes an attack between two units.
        /// </summary>
        /// <param name="data">The data describing a strike from the attacker.</param>
        /// <returns>
        /// The result of the attacking unit's strike, or null if the data was invalid.
        /// </returns>
        public CombatResult? Execute(AttackData data, UnitCombatData attacker, UnitCombatData target);

        /// <summary>
        /// Calculate combat data for a given unit.
        /// </summary>
        /// <param name="unit">The unit to parse.</param>
        /// <returns>The generated data.</returns>
        public UnitCombatData GetCombatData(IUnit unit);
    }
}
