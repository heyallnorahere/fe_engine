using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FEEngine
{
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
    public class Registry
    {
        public Registry()
        {
            mRegisters = new List<IRegister>();
        }
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
        public void CreateRegister<T>() where T : class, IRegisteredObject<T>
        {
            if (RegisterExists<T>())
            {
                throw new RegisterAlreadyExistsException();
            }
            mRegisters.Add(new Register<T>(this));
        }
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
                    mRegisters[i] = JsonSerializer.Deserialize<Register<T>>(jsonFilePath, (ref Newtonsoft.Json.JsonSerializer serializer) =>
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
            private Register<T> mParent;
        }
        public Register(Registry registry)
        {
            mElements = new List<T>();
            mRegistry = registry;
        }
        public bool IsOfType<U>() where U : class, IRegisteredObject<U>
        {
            return typeof(T) == typeof(U);
        }
        public int Count
        {
            get
            {
                return mElements.Count;
            }
        }
        public Registry Parent
        {
            get
            {
                return mRegistry;
            }
        }
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
        public void Add(T element)
        {
            element.SetRegister(Count, this);
            mElements.Add(element);
        }
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
        private List<T> mElements;
        private Registry mRegistry;
    }
}
