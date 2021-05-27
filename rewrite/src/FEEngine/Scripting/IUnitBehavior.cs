using System;
using System.Collections.Generic;
using System.Text;

namespace FEEngine.Scripting
{
    public interface IUnitBehavior
    {
        void Update();
        Unit Parent { get; set; }
    }
}
