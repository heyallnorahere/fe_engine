using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace FEEngine.Util
{
    public class ObjectRegistry
    {
        public static ObjectRegister<T> GetRegister<T>() where T : RegisteredObject<T>, new()
        {
            if (!RegisterExists<T>())
            {
                throw new Exception("Register does not exist!");
            }
            return new ObjectRegister<T>();
        }
        public static bool RegisterExists<T>() where T : RegisteredObject<T>, new()
        {
            return RegisterExists_Native(typeof(T));
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool RegisterExists_Native(Type type);
    }
}
