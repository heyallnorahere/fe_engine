using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FEEngine
{
    public class Weapon : Item
    {
        private Weapon(ulong parent, ulong inventoryIndex) : base(parent, inventoryIndex) { }
        public static new Weapon MakeFromInventoryIndex(Unit parent, ulong inventoryIndex)
        {
            return new Weapon(parent.Index, inventoryIndex);
        }
    }
}
