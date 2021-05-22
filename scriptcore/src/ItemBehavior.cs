using System;
using System.Collections.Generic;
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
            this.parent = new Item();
        }
    }
}
