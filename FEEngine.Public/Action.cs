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
                    if (unit.Map.IsOutOfBounds(newPosition))
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
                // todo: attack
                throw new NotImplementedException();
            }
        }

        public enum ID
        {
            Move,
            Attack
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
