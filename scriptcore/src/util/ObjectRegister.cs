using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace FEEngine.Util
{
    /// <summary>this is not a class to add objects to a register, rather one that creates wrapper objects</summary>
    public class ObjectRegister<T> where T : RegisteredObject<T>, new()
    {
        public T Get(ulong index)
        {
            T element = new T();
            element.SetRegisterIndex(index);
            return element;
        }
        public ulong Count { get { return GetCount_Native(typeof(T)); } }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern ulong GetCount_Native(Type type);
    }
}
