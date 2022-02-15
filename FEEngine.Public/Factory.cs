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
    public interface ICreationDesc
    {
        /// <summary>
        /// Verifies that all parameters are correct.
        /// </summary>
        /// <returns>If this structure is valid.</returns>
        public bool Verify();
    }

    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class FactoryInterfaceAttribute : Attribute
    {
        public FactoryInterfaceAttribute()
        {
            DescType = null;
        }

        public FactoryInterfaceAttribute(Type descType)
        {
            if (descType.IsPrimitive)
            {
                throw new ArgumentException("An object description type must not be a primitive!");
            }

            if (!descType.IsValueType)
            {
                throw new ArgumentException("An object description type must be a struct!");
            }

            var interfaces = new List<Type>(descType.GetInterfaces());
            if (!interfaces.Contains(typeof(ICreationDesc)))
            {
                throw new ArgumentException("All object description types must implement ICreationDesc!");
            }

            DescType = descType;
        }

        public Type? DescType { get; }
    }

    public abstract class Factory
    {
        private static Type? GetDescType(Type type, FactoryInterfaceAttribute attribute)
        {
            if (attribute.DescType != null)
            {
                return attribute.DescType;
            }

            string interfaceName = type.Name;
            if (interfaceName.StartsWith('I'))
            {
                string substring = interfaceName[1..];
                if (substring.Length > 0)
                {
                    string lower = substring.ToLower();
                    if (lower[0] != substring[0])
                    {
                        interfaceName = substring;
                    }
                }
            }

            string descTypeName = interfaceName + "Desc";
            if (type.Namespace != null)
            {
                descTypeName = type.Namespace + "." + descTypeName;
            }

            Type? descType = Type.GetType(descTypeName);
            if (descType != null)
            {
                var interfaces = new List<Type>(descType.GetInterfaces());
                if (interfaces.Contains(typeof(ICreationDesc)))
                {
                    return descType;
                }
            }

            return null;
        }

        private delegate T? CreateDelegate<T, D>(D desc) where T : class where D : struct, ICreationDesc;
        public Factory()
        {
            mCreationFunctions = new Dictionary<Type, Delegate>();

            RegisterFactoryType<IMap, MapDesc>(CreateMap);
            RegisterFactoryType<IUnit, UnitDesc>(CreateUnit);
            RegisterFactoryType<IItem, ItemDesc>(CreateItem);
        }

        private readonly Dictionary<Type, Delegate> mCreationFunctions;
        private void RegisterFactoryType<T, D>(CreateDelegate<T, D> func) where T : class where D : struct, ICreationDesc
        {
            Type keyType = typeof(T);
            if (mCreationFunctions.ContainsKey(keyType))
            {
                throw new ArgumentException("The given type is already registered!");
            }

            mCreationFunctions.Add(keyType, func);
        }

        public T? Create<T>(ICreationDesc desc) where T : class
        {
            Type type = typeof(T);
            if (!type.IsInterface)
            {
                return null;
            }

            var attribute = type.GetCustomAttribute<FactoryInterfaceAttribute>();
            if (attribute == null)
            {
                return null;
            }

            Type? descType = GetDescType(type, attribute);
            if (desc.GetType() != descType)
            {
                return null;
            }

            if (!desc.Verify())
            {
                return null;
            }

            if (!mCreationFunctions.ContainsKey(type))
            {
                return null;
            }

            Delegate func = mCreationFunctions[type];
            return (T?)func.DynamicInvoke(new object[] { desc });
        }

        protected abstract IMap? CreateMap(MapDesc desc);
        protected abstract IUnit? CreateUnit(UnitDesc desc);
        protected abstract IItem? CreateItem(ItemDesc desc);
    }
}