using System.Collections.Generic;
using FEEngine.Menus;

namespace FEEngine
{
    public interface IMenu : IRenderable
    {
        string GetTitle();
    }
    /// <summary>
    /// A controller for renderable menus
    /// </summary>
    public class UIController
    {
        /// <summary>
        /// Adds a new menu to the index of menus
        /// </summary>
        /// <typeparam name="T">The type of menu to add</typeparam>
        public static void AddMenu<T>() where T : IMenu, new()
        {
            if (!initialized)
            {
                return;
            }
            menus.Add(new T());
        }
        /// <summary>
        /// Adds an existing menu to the index of menus
        /// </summary>
        /// <param name="menu">The existing menu to add</param>
        public static void AddMenu(IMenu menu)
        {
            if (!initialized)
            {
                return;
            }
            menus.Add(menu);
        }
        /// <summary>
        /// Finds the first menu of the specified type
        /// </summary>
        /// <typeparam name="T">The type of menu to find</typeparam>
        /// <returns>A menu of the specified type, or null if none was found</returns>
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
        /// <summary>
        /// The unit selected by the <see cref="Player"/> instance, to be used by the <see cref="UnitContextMenu"/>
        /// </summary>
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
        /// <summary>
        /// Whether or not the <see cref="UnitContextMenu"/> is open
        /// </summary>
        public static bool IsUnitContextMenuOpen { get; set; }
        /// <summary>
        /// Unselects the object referenced by <see cref="SelectedUnit"/>
        /// </summary>
        public static void ResetSelectedUnit()
        {
            selectedUnitIndex = -1;
        }
        /// <summary>
        /// Initializes the <see cref="UIController"/> class
        /// </summary>
        /// <param name="game">The instance of the <see cref="Game"/> class</param>
        public static void Init(Game game)
        {
            gameInstance = game;
            IsUnitContextMenuOpen = false;
            initialized = true;
            AddMenu(new UnitContextMenu());
            AddMenu(new TileInfoMenu());
        }
        internal static Game GameInstance { get => gameInstance; }
        private static Game gameInstance;
        private static bool initialized = false;
        private static readonly List<IMenu> menus = new();
        private static int selectedUnitIndex = -1;
    }
}