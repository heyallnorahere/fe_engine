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
using System.Runtime;

namespace FEEngine
{
    public interface ICreationDesc
    {
        /// <summary>
        /// Verifies that all parameters are correct.
        /// </summary>
        /// <returns>If this structure is valid.</returns>
        public bool Verify();

        /// <summary>
        /// Clones the structure.
        /// </summary>
        /// <returns>The resulting object.</returns>
        public ICreationDesc Clone();
    }

    /// <summary>
    /// An attribute required for all interfaces to be implemented by a <see cref="Factory"/>.
    /// </summary>
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

    /// <summary>
    /// A factory prototype creates an object from static data.
    /// A prototype can be created by a <see cref="Factory"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The interface of which objects 
    /// created by this prototype implement.
    /// </typeparam>
    public sealed class FactoryPrototype<T> where T : class
    {
        internal FactoryPrototype(ICreationDesc baseDesc, Factory factory)
        {
            mBaseDesc = baseDesc;
            mFactory = factory;
        }

        /// <summary>
        /// Instantiates the prototype.
        /// </summary>
        /// <param name="dataCallback">Sets all dynamic data for object creation.</param>
        /// <returns>The created object, or null if the function failed.</returns>
        public T? Instantiate(PrototypeDelegate? dataCallback = null)
        {
            ICreationDesc desc = mBaseDesc.Clone();
            dataCallback?.Invoke(ref desc);
            return mFactory.Create<T>(desc);
        }

        private readonly ICreationDesc mBaseDesc;
        private readonly Factory mFactory;
    }
    public delegate void PrototypeDelegate(ref ICreationDesc desc);

    /// <summary>
    /// A factory creates object that implement interfaces with the
    /// <see cref="FactoryInterfaceAttribute">FactoryInterface</see> attribute.
    /// </summary>
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

        public Factory()
        {
            mCreationFunctions = new Dictionary<Type, Delegate>();
            RegisterFactoryTypes();
        }

        private readonly Dictionary<Type, Delegate> mCreationFunctions;
        protected delegate T? CreateDelegate<T, D>(D desc) where T : class where D : struct, ICreationDesc;

        /// <summary>
        /// A function called on creation. Should call
        /// <see cref="RegisterFactoryType{T, D}(CreateDelegate{T, D})">RegisterFactoryType</see>
        /// for every type registered.
        /// </summary>
        protected abstract void RegisterFactoryTypes();

        /// <summary>
        /// Registers a creation callback to be
        /// called by <see cref="Create{T}(ICreationDesc)">Create</see>.
        /// </summary>
        /// <typeparam name="T">The interface of which the created object implements.</typeparam>
        /// <typeparam name="D">The type of data to be passed.</typeparam>
        /// <param name="func">The callback to be registered.</param>
        /// <returns>Whether the function succeeded.</returns>
        protected bool RegisterFactoryType<T, D>(CreateDelegate<T, D> func) where T : class where D : struct, ICreationDesc
        {
            Type keyType = typeof(T);
            if (mCreationFunctions.ContainsKey(keyType) || !keyType.IsInterface)
            {
                return false;
            }

            var attribute = keyType.GetCustomAttribute<FactoryInterfaceAttribute>();
            if (attribute == null || GetDescType(keyType, attribute) != typeof(D))
            {
                return false;
            }

            mCreationFunctions.Add(keyType, func);
            return true;
        }

        /// <summary>
        /// Creates an object of the specified interface type.
        /// </summary>
        /// <typeparam name="T">The interface of which the object needs to implement.</typeparam>
        /// <param name="desc">The data to be passed to the underlying implementation.</param>
        /// <returns>The created object, or null if this function failed.</returns>
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
            return (T?)func.DynamicInvoke(desc);
        }
        
        /// <summary>
        /// Creates a prototype that stores data for creation of multiple objects with the same data.
        /// </summary>
        /// <typeparam name="T">The interface that each object created will implement.</typeparam>
        /// <param name="baseDesc">The base data to create the object with.</param>
        /// <returns>The created prototype.</returns>
        public FactoryPrototype<T>? CreatePrototype<T>(ICreationDesc baseDesc) where T : class
        {
            Type type = typeof(T);
            if (!mCreationFunctions.ContainsKey(type) || !type.IsInterface)
            {
                return null;
            }

            var attribute = type.GetCustomAttribute<FactoryInterfaceAttribute>();
            if (attribute == null || GetDescType(type, attribute) != baseDesc.GetType())
            {
                return null;
            }

            return new FactoryPrototype<T>(baseDesc, this);
        }
    }
}