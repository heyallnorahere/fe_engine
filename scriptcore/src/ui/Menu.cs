using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Reflection;
using FEEngine.Util;

namespace FEEngine.UI
{
    public class Menu : RegisteredObject<Menu>
    {
        public ulong Index { get; private set; }
        public List<MenuItem> GetMenuItems()
        {
            ulong count = GetMenuItemCount_Native(this.Index);
            List<MenuItem> items = new List<MenuItem>();
            for (ulong i = 0; i < count; i++)
            {
                items.Add(GetMenuItem_Native(this.Index, i));
            }
            return items;
        }
        public void AddMenuItem(MenuItem item)
        {
            AddMenuItem_Native(this.Index, item);
        }
        public Menu()
        {
            this.Index = 0;
        }
        public void SetRegisterIndex(ulong index)
        {
            this.Index = index;
        }
        public static Menu MakeFromRegisterIndex(ulong index)
        {
            Menu menu = new Menu();
            menu.Index = index;
            return menu;
        }
        public static Menu MakeNew()
        {
            return MakeFromRegisterIndex(MakeNew_Native());
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern ulong MakeNew_Native();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern ulong GetMenuItemCount_Native(ulong index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern MenuItem GetMenuItem_Native(ulong index, ulong itemIndex);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void AddMenuItem_Native(ulong index, MenuItem item);
    }
}
