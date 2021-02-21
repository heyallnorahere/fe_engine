using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FEEngine
{
    public class Behavior
    {
        protected Unit Parent { get { return this.parent; } }
        private Unit parent;
        protected Behavior()
        {
            parent = Unit.MakeFromIndex(0);
        }
    }
}
