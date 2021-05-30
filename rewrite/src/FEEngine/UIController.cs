using System.Collections.Generic;
using FEEngine.Math;
using FEEngine.Menus;

namespace FEEngine
{
    public class UIController
    {
        public static void AddMenu<T>() where T : IRenderable, new()
        {
            if (!initialized)
            {
                return;
            }
            menus.Add(new T());
        }
        public static void AddMenu(IRenderable menu)
        {
            if (!initialized)
            {
                return;
            }
            menus.Add(menu);
        }
        public static T FindMenu<T>() where T : class, IRenderable
        {
            T menu = null;
            if (initialized)
            {
                foreach (IRenderable element in menus)
                {
                    if (element.GetType() == typeof(T))
                    {
                        menu = (T)element;
                        break;
                    }
                }
            }
            return menu;
        }
        public static Unit SelectedUnit
        {
            get
            {
                Register<Unit> unitRegister = gameInstance.Registry.GetRegister<Unit>();
                if (selectedUnitIndex == -1)
                {
                    return null;
                }
                return unitRegister[selectedUnitIndex];
            }
            set
            {
                selectedUnitIndex = value.RegisterIndex;
            }
        }
        public static bool IsUnitContextMenuOpen { get; set; }
        public static void ResetSelectedUnit()
        {
            selectedUnitIndex = -1;
        }
        public static void Init(Game game)
        {
            gameInstance = game;
            IsUnitContextMenuOpen = false;
            initialized = true;
            AddMenu<UnitContextMenu>();
        }
        private static Game gameInstance;
        private static bool initialized = false;
        private static readonly List<IRenderable> menus = new();
        private static int selectedUnitIndex = -1;
    }
}