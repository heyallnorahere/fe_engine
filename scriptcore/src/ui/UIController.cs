using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using FEEngine.Util;

namespace FEEngine.UI
{
    public class UIController : RegisteredObject<UIController>
    {
        public struct MenuDescriptionStruct
        {
            public String name;
            public Menu menu;
        }
        public ulong Index { get; private set; }
        public static UIController MakeFromRegisterIndex(ulong index)
        {
            UIController uiController = new UIController();
            uiController.Index = index;
            return uiController;
        }
        public UIController()
        {
            this.Index = 0;
        }
        public void SetRegisterIndex(ulong index)
        {
            this.Index = index;
        }
        public Unit GetUnitMenuTarget()
        {
            if (!this.HasUnitSelected())
            {
                throw new Exception("No unit has been selected!");
            }
            return Unit.MakeFromIndex(GetUnitMenuTarget_Native(this.Index));
        }
        public bool HasUnitSelected()
        {
            return HasUnitSelected_Native(this.Index);
        }
        public List<MenuDescriptionStruct> GetUserMenus()
        {
            List<MenuDescriptionStruct> menus = new List<MenuDescriptionStruct>();
            ulong count = GetUserMenuCount_Native(this.Index);
            for (ulong i = 0; i < count; i++)
            {
                menus.Add(GetUserMenu_Native(this.Index, i));
            }
            return menus;
        }
        public void ExitUnitMenu()
        {
            ExitUnitMenu_Native(this.Index);
        }
        public void AddUserMenu(MenuDescriptionStruct menu)
        {
            AddUserMenu_Native(this.Index, menu);
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern ulong GetUnitMenuTarget_Native(ulong index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool HasUnitSelected_Native(ulong index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern ulong GetUserMenuCount_Native(ulong index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern MenuDescriptionStruct GetUserMenu_Native(ulong index, ulong menuIndex);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void AddUserMenu_Native(ulong index, MenuDescriptionStruct menu);
        [MethodImpl(MethodImplOptions.InternalCall)]

        private static extern void ExitUnitMenu_Native(ulong index);
    }
}
