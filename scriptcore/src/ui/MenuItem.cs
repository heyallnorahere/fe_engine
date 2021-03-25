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
        internal static String GetActionName(MenuItemAction action)
        {
            String methodName = action.Method.Name;
            String className = action.Method.ReflectedType.FullName;
            return className + "." + methodName;
        }
        internal static MenuItemAction MakeAction(Type type, string name)
        {
            Type[] types = new Type[] { typeof(UIController) };
            MethodInfo info = type.GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic, null, types, null);
            MenuItemAction action = (MenuItemAction)Delegate.CreateDelegate(typeof(MenuItemAction), info);
            return action;
        }
    }
}
