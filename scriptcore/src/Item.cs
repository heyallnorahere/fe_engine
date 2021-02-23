using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
namespace FEEngine
{
    public class Item
    {
        public ulong Index { get; private set; }
        public Unit Parent { get { return Unit.MakeFromIndex(this.parentIndex); } }
        protected ulong parentIndex;
        public String Name 
        {
            get
            {
                return GetName_Native(this.parentIndex, this.Index);
            }
            set
            {
                SetName_Native(this.parentIndex, this.Index, value);
            }
        }
        public void Use()
        {
            Use_Native(this.parentIndex, this.Index);
        }
        protected Item(ulong parent, ulong inventoryIndex)
        {
            this.Index = inventoryIndex;
            this.parentIndex = parent;
        }
        public static Item MakeFromInventoryIndex(Unit parent, ulong inventoryIndex)
        {
            return new Item(parent.Index, inventoryIndex);
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern String GetName_Native(ulong unit, ulong index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetName_Native(ulong unit, ulong index, String name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Use_Native(ulong unit, ulong index);
    }
}
