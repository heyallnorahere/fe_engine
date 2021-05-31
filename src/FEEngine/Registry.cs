using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FEEngine
{
    /// <summary>
    /// An interface to an object stored in a <see cref="Register{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of object</typeparam>
    [JsonObject]
    public interface IRegisteredObject<T> where T : class, IRegisteredObject<T>
    {
        int RegisterIndex { get; }
        Register<T> GetRegister();
        void SetRegister(int index, Register<T> register);
    }
    // todo: add content to these classes
    public class RegisterDoesNotExistException : Exception
    {
        public RegisterDoesNotExistException() : base("The specified register does not exist") { }
    }
    public class RegisterAlreadyExistsException : Exception
    {
        public RegisterAlreadyExistsException() : base("The specified register already exists") { }
    }
    /// <summary>
    /// An index of <see cref="Register{T}"/> objects
    /// </summary>
    public class Registry
    {
        public Registry()
        {
            mRegisters = new List<IRegister>();
        }
        /// <summary>
        /// Checks if the <see cref="Register{T}"/> of the specified type exists
        /// </summary>
        /// <typeparam name="T">The type of object that the register contains</typeparam>
        /// <returns>If the specified <see cref="Register{T}"/> exists</returns>
        public bool RegisterExists<T>() where T : class, IRegisteredObject<T>
        {
            foreach (IRegister register in mRegisters)
            {
                if (register?.IsOfType<T>() ?? false)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Retrieves the <see cref="Register{T}"/> of the specified type
        /// </summary>
        /// <typeparam name="T">The type of object that the register contains</typeparam>
        /// <returns>The specified <see cref="Register{T}"/></returns>
        /// <exception cref="RegisterDoesNotExistException"/>
        public Register<T> GetRegister<T>() where T : class, IRegisteredObject<T>
        {
            if (!RegisterExists<T>())
            {
                throw new RegisterDoesNotExistException();
            }
            for (int i = 0; i < mRegisters.Count; i++)
            {
                IRegister register = mRegisters[i];
                if (register.IsOfType<T>())
                {
                    return (Register<T>)register;
                }
            }
            return null;
        }
        /// <summary>
        /// Creates a register of the specified type
        /// </summary>
        /// <typeparam name="T">The type of object the register will hold</typeparam>
        /// <exception cref="RegisterAlreadyExistsException"/>
        public void CreateRegister<T>() where T : class, IRegisteredObject<T>
        {
            if (RegisterExists<T>())
            {
                throw new RegisterAlreadyExistsException();
            }
            mRegisters.Add(new Register<T>(this));
        }
        /// <summary>
        /// Serializes a register into a JSON file
        /// </summary>
        /// <typeparam name="T">The type of object the serialized register holds</typeparam>
        /// <param name="jsonFilePath">The output file's path</param>
        /// <exception cref="RegisterDoesNotExistException"/>
        public void SerializeRegister<T>(string jsonFilePath) where T : class, IRegisteredObject<T>
        {
            if (!RegisterExists<T>())
            {
                throw new RegisterDoesNotExistException();
            }
            foreach (IRegister register in mRegisters)
            {
                if (register?.IsOfType<T>() ?? false)
                {
                    JsonSerializer.Serialize(jsonFilePath, (Register<T>)register);
                    break;
                }
            }
        }
        private class Converter<T> : CustomCreationConverter<Register<T>> where T : class, IRegisteredObject<T>
        {
            public Converter(Registry parent)
            {
                mParent = parent;
            }
            public override Register<T> Create(Type objectType)
            {
                return new Register<T>(mParent);
            }
            private Registry mParent;
        }
        /// <summary>
        /// Deserializes a register from a JSON file
        /// </summary>
        /// <typeparam name="T">The type of object the deserialized register will hold</typeparam>
        /// <param name="jsonFilePath">The input file's path</param>
        public void DeserializeRegister<T>(string jsonFilePath) where T : class, IRegisteredObject<T>
        {
            if (!RegisterExists<T>())
            {
                CreateRegister<T>();
            }
            for (int i = 0; i < mRegisters.Count; i++)
            {
                if (mRegisters[i].IsOfType<T>())
                {
                    mRegisters[i] = JsonSerializer.Deserialize<Register<T>>(jsonFilePath, (Newtonsoft.Json.JsonSerializer serializer) =>
                    {
                        serializer.Converters.Add(new Converter<T>(this));
                    });
                }
            }
        }
        private readonly List<IRegister> mRegisters;
    }
    internal interface IRegister
    {
        bool IsOfType<T>() where T : class, IRegisteredObject<T>;
    }
    /// <summary>
    /// A glorified <see cref="List{T}"/> of <see cref="IRegisteredObject{T}"/> objects
    /// </summary>
    /// <typeparam name="T">The type of <see cref="IRegisteredObject{T}"/> the register holds</typeparam>
    [JsonArray]
    public class Register<T> : IRegister, IEnumerable<T>, ICollection<T> where T : class, IRegisteredObject<T>
    {
        private struct Enumerator : IEnumerator<T>
        {
            public bool MoveNext()
            {
                if (mPosition < mParent.Count - 1)
                {
                    mPosition++;
                    return true;
                }
                return false;
            }
            public void Reset()
            {
                mPosition = -1;
            }
            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }
            public T Current
            {
                get
                {
                    return mParent[mPosition];
                }
            }
            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }
            public Enumerator(Register<T> parent)
            {
                mPosition = -1;
                mParent = parent;
            }
            private int mPosition;
            private readonly Register<T> mParent;
        }
        public Register(Registry registry)
        {
            mElements = new List<T>();
            mRegistry = registry;
        }
        /// <summary>
        /// Checks if this register holds objects of the specified type
        /// </summary>
        /// <typeparam name="U">The type of <see cref="IRegisteredObject{T}"/> the register is checking against</typeparam>
        /// <returns>See summary</returns>
        public bool IsOfType<U>() where U : class, IRegisteredObject<U>
        {
            return typeof(T).FullName == typeof(U).FullName;
        }
        /// <summary>
        /// The number of objects currently stored
        /// </summary>
        public int Count
        {
            get
            {
                return mElements.Count;
            }
        }
        /// <summary>
        /// The parent <see cref="Registry"/>
        /// </summary>
        public Registry Parent
        {
            get
            {
                return mRegistry;
            }
        }
        /// <summary>
        /// Retrieves an object at the specified index
        /// </summary>
        /// <param name="index">The index to search at</param>
        /// <returns>The object found</returns>
        /// <exception cref="IndexOutOfRangeException"/>
        public T this[int index]
        {
            get
            {
                if (index >= Count)
                {
                    throw new IndexOutOfRangeException();
                }
                return mElements[index];
            }
            set
            {
                mElements[index] = value;
            }
        }
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// Adds an object to the <see cref="Register{T}"/>
        /// </summary>
        /// <param name="element"></param>
        public void Add(T element)
        {
            element.SetRegister(Count, this);
            mElements.Add(element);
        }
        /// <summary>
        /// Clears the objects from the <see cref="Register{T}"/>
        /// </summary>
        public void Clear()
        {
            mElements.Clear();
        }
        public bool Contains(T item)
        {
            return mElements.Contains(item);
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            mElements.CopyTo(array, arrayIndex);
        }
        public bool Remove(T item)
        {
            return mElements.Remove(item);
        }
        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        private readonly List<T> mElements;
        private readonly Registry mRegistry;
    }
}
