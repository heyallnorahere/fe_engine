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
        }
        public String name;
        public MenuItemType type;
        public MenuItemAction action;
        public ulong submenuIndex;
    }
}
