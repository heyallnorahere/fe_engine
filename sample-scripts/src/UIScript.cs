using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FEEngine;
using FEEngine.UI;

namespace Scripts
{
    public class UIScript
    {
        private static void KillUnit(UIController uiController)
        {
            uiController.GetUnitMenuTarget().HP = 0;
            uiController.ExitUnitMenu();
        }
        public void Initialize(UIController uiController)
        {
            var menuDescriptionStruct = new UIController.MenuDescriptionStruct();
            menuDescriptionStruct.name = "Kill Unit";
            menuDescriptionStruct.menu = Menu.MakeNew();
            MenuItem item = new MenuItem();
            item.name = "Are you sure?";
            item.type = MenuItem.MenuItemType.NOACTION;
            menuDescriptionStruct.menu.AddMenuItem(item);
            item = new MenuItem();
            item.name = "Yes";
            item.type = MenuItem.MenuItemType.ACTION;
            item.action = KillUnit;
            menuDescriptionStruct.menu.AddMenuItem(item);
            item = new MenuItem();
            item.name = "No";
            item.type = MenuItem.MenuItemType.BACK;
            menuDescriptionStruct.menu.AddMenuItem(item);
            Menu secondMenu = Menu.MakeNew();
            item = new MenuItem();
            item.name = "Back";
            item.type = MenuItem.MenuItemType.BACK;
            secondMenu.AddMenuItem(item);
            item = new MenuItem();
            item.name = "Third option";
            item.type = MenuItem.MenuItemType.SUBMENU;
            item.SetSubmenu(secondMenu);
            menuDescriptionStruct.menu.AddMenuItem(item);
            uiController.AddUserMenu(menuDescriptionStruct);
        }
    }
}
