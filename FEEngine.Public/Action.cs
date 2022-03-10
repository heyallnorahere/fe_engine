﻿/*
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
    public struct AttackActionArgs
    {
        public AttackActionArgs()
        {
            RoundData = null;
            Results = null;
        }

        public RoundData? RoundData;
        public IList<CombatResult?>? Results;
    };

    public struct ItemActionArgs
    {
        public ItemActionArgs()
        {
            InventoryIndex = -1;
            ItemUses = 1;
            Unusable = null;
        }

        public int InventoryIndex, ItemUses;
        public ValueWrapper<bool>? Unusable;
    }

    public sealed class Action
    {
        private static class IDCallbacks
        {
            public static bool Move(IUnit unit, object? data)
            {
                if (unit.Map == null)
                {
                    return false;
                }

                if (data is Vector offset)
                {
                    var newPosition = unit.Position + offset;
                    int movement = unit.Stats.Movement;

                    if (unit.Map.IsOutOfBounds(newPosition) || newPosition.TaxicabLength > movement)
                    {
                        return false;
                    }

                    unit.Position = newPosition;
                    return true;
                }

                return false;
            }

            public static bool Attack(IUnit unit, object? data)
            {
                if (data is AttackActionArgs args)
                {
                    if (args.RoundData == null)
                    {
                        return false;
                    }

                    var roundData = args.RoundData.Value;
                    if (roundData.Attacker.Unit != unit)
                    {
                        return false;
                    }

                    if (!roundData.Engine.CanAttack(roundData.Attacker, roundData.Target))
                    {
                        return false;
                    }

                    foreach (int index in roundData.Indices)
                    {
                        var attackData = roundData.Data[index];

                        UnitCombatData currentAttacker, currentTarget;
                        if (attackData.Counter)
                        {
                            currentAttacker = roundData.Target;
                            currentTarget = roundData.Attacker;
                        }
                        else
                        {
                            currentAttacker = roundData.Attacker;
                            currentTarget = roundData.Target;
                        }

                        var result = roundData.Engine.Execute(attackData, currentAttacker, currentTarget);
                        if (args.Results != null)
                        {
                            args.Results.Add(result);
                        }

                        if (!roundData.Engine.CanAttack(currentAttacker, currentTarget))
                        {
                            break;
                        }
                    }

                    return true;
                }

                return false;
            }

            public static bool Item(IUnit unit, object? data)
            {
                if (data is ItemActionArgs args)
                {
                    if (args.ItemUses <= 0)
                    {
                        return false;
                    }

                    if (args.InventoryIndex < 0 || args.InventoryIndex >= unit.Inventory.Count)
                    {
                        return false;
                    }

                    IItem item = unit.Inventory[args.InventoryIndex];
                    if (item.Behavior == null)
                    {
                        return false;
                    }

                    if (item.UsesRemaining <= 0)
                    {
                        return false;
                    }

                    item.Behavior(item, unit);
                    bool unusable = item.OnItemUse(args.ItemUses);
                    if (args.Unusable != null)
                    {
                        args.Unusable.Value = unusable;
                    }

                    return true;
                }

                return false;
            }
        }

        public enum ID
        {
            Move,
            Attack,
            Item
        }

        /// <summary>
        /// A callback that an <see cref="Action"/> will call.
        /// </summary>
        /// <param name="unit">The calling unit.</param>
        /// <param name="data">The data passed to <see cref="Create(string, object?)"/>.</param>
        /// <returns>If the action succeeded.</returns>

        public delegate bool Callback(IUnit unit, object? data);

        static Action()
        {
            mRegisteredActions = new Dictionary<string, Callback>();

            Type callbacksClass = typeof(IDCallbacks);
            foreach (ID id in Enum.GetValues<ID>())
            {
                string name = id.ToString();
                MethodInfo? method = callbacksClass.GetMethod(name);

                if (method != null)
                {
                    var callback = (Callback)Delegate.CreateDelegate(typeof(Callback), method);
                    Register(name, callback);
                }
            }
        }

        public static bool Register(string name, Callback callback)
        {
            if (mRegisteredActions.ContainsKey(name))
            {
                return false;
            }

            mRegisteredActions.Add(name, callback);
            return true;
        }

        /// <summary>
        /// Creates an action from an <see cref="ID"/>.
        /// </summary>
        /// <param name="id">The ID of the action.</param>
        /// <param name="data">The data that will be passed.</param>
        /// <returns>The created action.</returns>
        public static Action? Create(ID id, object? data) => Create(id.ToString(), data);

        /// <summary>
        /// Creates an action that references a callback registered via <see cref="Register(string, Callback)"/>.
        /// </summary>
        /// <param name="name">The name of the callback.</param>
        /// <param name="data">The data to be passed.</param>
        /// <returns>The created action.</returns>
        public static Action? Create(string name, object? data)
        {
            Action? action = null;
            if (mRegisteredActions.ContainsKey(name))
            {
                var callback = mRegisteredActions[name];
                action = new Action(callback, data);
            }

            return action;
        }

        private Action(Callback callback, object? data)
        {
            mCallback = callback;
            mData = data;
        }

        /// <summary>
        /// Invoke this action.
        /// </summary>
        /// <param name="unit">The unit calling.</param>
        public bool Invoke(IUnit unit) => mCallback(unit, mData);

        private readonly Callback mCallback;
        private readonly object? mData;

        private static readonly Dictionary<string, Callback> mRegisteredActions;
    }
}
