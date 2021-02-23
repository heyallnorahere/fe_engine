using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEEngine
{
    public class ItemBehavior
    {
        protected Item Parent { get { return this.parent; } }
        private Item parent;
        protected ItemBehavior()
        {
            this.parent = Map.GetUnit(0).GetInventoryItem(0);
        }
    }
}
