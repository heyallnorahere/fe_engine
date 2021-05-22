using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using FEEngine.Util;

namespace FEEngine
{
    public class Item : RegisteredObject<Item>
    {
        public ulong Index { get; private set; }
        public Unit Parent { get { return Unit.MakeFromIndex(this.parentIndex); } }
        protected ulong parentIndex;
        public string Name
        {
            get
            {
                return GetName_Native(this.Index);
            }
            set
            {
                SetName_Native(this.Index, value);
            }
        }
        public void Use()
        {
            if (this.IsWeapon())
            {
                return;
            }
            Use_Native(this.parentIndex, this.Index);
        }
        public bool IsWeapon()
        {
            return IsWeapon_Native(this.Index);
        }
        public void SetParent(Unit unit)
        {
            this.parentIndex = unit.Index;
        }
        public void SetRegisterIndex(ulong index)
        {
            this.Index = index;
        }
        protected Item(ulong index)
        {
            this.Index = index;
        }
        public Item()
        {
            this.Index = 0;
            this.parentIndex = ObjectRegistry.GetRegister<Unit>().Count;
        }
        public static Item MakeFromRegistryIndex(ulong index, ulong parent)
        {
            Item item = new Item(index);
            item.parentIndex = parent;
            return item;
        }
        public static Item NewItem<Behavior>(string name) where Behavior : ItemBehavior
        {
            ulong index = NewItem_Native(name, typeof(Behavior));
            Item item = new Item(index);
            return item;
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string GetName_Native(ulong index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetName_Native(ulong index, string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Use_Native(ulong unit, ulong index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool IsWeapon_Native(ulong index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern ulong NewItem_Native(string name, Type behaviorType);
    }
}
