using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FEEngine.UI
{
    public struct MenuItem
    {
        public delegate void MenuItemAction(UIController uiController);
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
