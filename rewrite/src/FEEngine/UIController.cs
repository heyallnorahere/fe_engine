using System.Collections.Generic;
using FEEngine.Math;

namespace FEEngine
{
    public interface IMenu
    {
        public IVec2<int> Size { get; }
        public void Render(IVec2<int> originPoint);
    }
    public class UIController
    {
        public struct RenderAgent : IRenderable
        {
            public void Render()
            {
                if (!initialized)
                {
                    return;
                }
                RenderMenus();
            }
        }
        public static void AddMenu<T>() where T : IMenu, new()
        {
            if (!initialized)
            {
                return;
            }
            menus.Add(new T());
        }
        public static void AddMenu(IMenu menu)
        {
            if (!initialized)
            {
                return;
            }
            menus.Add(menu);
        }
        public static T FindMenu<T>() where T : class, IMenu
        {
            T menu = null;
            if (initialized)
            {
                foreach (IMenu element in menus)
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
        public static void ResetSelectedUnit()
        {
            selectedUnitIndex = -1;
        }
        private static void RenderMenus()
        {
            var positionDict = GetPositionDict();
            // todo: render hashtag border
            foreach (KeyValuePair<IMenu, IVec2<int>> pair in positionDict)
            {
                pair.Key.Render(pair.Value);
            }
        }
        private static Dictionary<IMenu, IVec2<int>> GetPositionDict()
        {
            Dictionary<IMenu, IVec2<int>> positionDict = new();
            // todo: iterate through calculate where each menu is rendered
            // each menu should have a space in between, for a hashtag border
            return positionDict;
        }
        public static void Init(Game game)
        {
            gameInstance = game;
            initialized = true;
        }
        private static Game gameInstance;
        private static bool initialized = false;
        private static readonly List<IMenu> menus = new();
        private static int selectedUnitIndex = -1;
    }
}
