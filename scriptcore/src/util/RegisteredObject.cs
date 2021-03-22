using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEEngine.Util
{
    public interface RegisteredObject<T> where T : new()
    {
        public void SetRegisterIndex(ulong index);
    }
}
