using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FEEngine
{
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
                if (register.IsOfType<T>())
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
        private readonly List<IRegister> mRegisters;
    }
    internal interface IRegister
    {
        bool IsOfType<T>() where T : class, IRegisteredObject<T>;
    }
    [JsonArray(false)]
    public class Register<T> : IRegister where T : class, IRegisteredObject<T>
    {
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
        public void Add(T element)
        {
            element.SetRegister(Count, this);
            mElements.Add(element);
        }
        private List<T> mElements;
        private Registry mRegistry;
    }
}
