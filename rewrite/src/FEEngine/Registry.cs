using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;

namespace FEEngine
{
    public interface IRegisteredObject<T> where T : class, IRegisteredObject<T>
    {
        int RegisterIndex { get; }
        Register<T> GetRegister();
        void SetRegister(int index, Register<T> register);
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
                throw new Exception("The specified register does not exist!");
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
                throw new Exception("The specified register already exists!");
            }
            mRegisters.Add(new Register<T>());
        }
        private readonly List<IRegister> mRegisters;
    }
    interface IRegister
    {
        bool IsOfType<T2>() where T2 : class, IRegisteredObject<T2>;
    }
    /// <summary>
    /// Essentially a glorified List<>
    /// </summary>
    /// <typeparam name="T">The type of data to store</typeparam>
    public class Register<T> : IRegister where T : class, IRegisteredObject<T>
    {
        public Register()
        {
            mElements = new List<T>();
        }
        public bool IsOfType<T2>() where T2 : class, IRegisteredObject<T2>
        {
            return true;
        }
        public int Count
        {
            get
            {
                return mElements.Count;
            }
        }
        public T this[int index]
        {
            get
            {
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
    }
}
