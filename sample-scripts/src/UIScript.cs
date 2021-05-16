using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FEEngine;
using FEEngine.UI;
using FEEngine.Util;
using FEEngine.Math;

namespace Scripts
{
    public class UIScript
    {
        private static void KillUnit(UIController uiController)
        {
            uiController.GetUnitMenuTarget().HP = 0;
            uiController.ExitUnitMenu();
        }
        private static void LogTile(UIController uiController)
        {
            Unit unit = uiController.GetUnitMenuTarget();
            bool match = false;
            Vec2<int> position = new Vec2<int>(0, 0);
            Map map = Map.GetMap();
            Vec2<int> mapSize = map.GetSize();
            for (int x = 0; x < mapSize.X && !match; x++)
            {
                for (int y = 0; y < mapSize.Y; y++)
                {
                    Vec2<int> pos = new Vec2<int>(x, y);
                    try
                    {
                        Unit currentUnit = map.GetUnitAt(pos);
                        if (currentUnit.Index == unit.Index)
                        {
                            match = true;
                            position = pos;
                            break;
                        }
                    }
                    catch (Exception) { }
                }
            }
            if (match)
            {
                Tile tile = map.GetTileAt(position);
                String format = "Tile at position: ({0}, {1})\nPASSING PROPERTIES:\nFoot: {2}\n\nColor: {3}\n";
                String foot = tile.PassingProperties.Foot.ToString();
                String color = tile.Color.ToString();
                String output = String.Format(format, position.X.ToString(), position.Y.ToString(), foot, color);
                Logger.Print(output);
            }
            else
            {
                throw new Exception("Unit selected was not found on the map!");
            }
            uiController.ExitUnitMenu();
        }
        private static MenuItem MakeMenuItem(String name, MenuItem.MenuItemType type, MenuItemAction action = null, Menu menu = null)
        {
            MenuItem item = new MenuItem();
            item.name = name;
            item.type = type;
            if (action != null)
            {
                item.action = action;
            }
            if (menu != null)
            {
                item.SetSubmenu(menu);
            }
            return item;
        }
        public void Initialize(UIController uiController)
        {
            var menuDescriptionStruct = new UIController.MenuDescriptionStruct();
            menuDescriptionStruct.name = "Kill Unit";
            menuDescriptionStruct.menu = Menu.MakeNew();
            menuDescriptionStruct.menu.AddMenuItem(MakeMenuItem("Are you sure?", MenuItem.MenuItemType.NOACTION));
            menuDescriptionStruct.menu.AddMenuItem(MakeMenuItem("Yes", MenuItem.MenuItemType.ACTION, KillUnit));
            menuDescriptionStruct.menu.AddMenuItem(MakeMenuItem("No", MenuItem.MenuItemType.BACK));
            Menu secondMenu = Menu.MakeNew();
            secondMenu.AddMenuItem(MakeMenuItem("Tile test", MenuItem.MenuItemType.ACTION, LogTile));
            secondMenu.AddMenuItem(MakeMenuItem("Back", MenuItem.MenuItemType.BACK));
            menuDescriptionStruct.menu.AddMenuItem(MakeMenuItem("Third option", MenuItem.MenuItemType.SUBMENU, menu: secondMenu));
            uiController.AddUserMenu(menuDescriptionStruct);
        }
    }
}
