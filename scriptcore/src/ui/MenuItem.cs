using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using FEEngine.Util;

namespace FEEngine.UI
{
    public delegate void MenuItemAction(UIController uiController);
    public struct MenuItem
    {
        public enum MenuItemType
        {
            SUBMENU,
            ACTION,
            BACK,
            NOACTION,
        }
        public string name;
        public MenuItemType type;
        public MenuItemAction action;
        public ulong submenuIndex;
        public void SetSubmenu(Menu menu)
        {
            this.submenuIndex = menu.Index;
        }
    }
}
