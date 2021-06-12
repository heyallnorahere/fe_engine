using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using FEEngine.Menus;

namespace FEEngine
{
    /// <summary>
    /// An object that oversees the entire game's state
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Whether the game is in debug mode; enables functions such as <see cref="BreakDebugger"/>
        /// </summary>
        public static bool Debug
        {
            get => debug;
            set => debug = value;
        }
        /// <summary>
        /// Whether the game is being run from the C++ host application
        /// </summary>
        public static bool HasNativeImplementation
        {
            get => hasNativeImplementation;
            set => hasNativeImplementation = value;
        }
        /// <summary>
        /// Breaks the debugger; only available when both <see cref="Debug"/> and <see cref="HasNativeImplementation"/> are set to "true"
        /// </summary>
        public static void BreakDebugger()
        {
            if (Debug && HasNativeImplementation)
            {
                BreakDebugger_Native();
            }
        }
        /// <summary>
        /// The game's <see cref="FEEngine.PhaseManager"/>
        /// </summary>
        public PhaseManager PhaseManager { get; private set; }
        /// <summary>
        /// The index of the current <see cref="Map"/> that is being played
        /// </summary>
        public int CurrentMapIndex { get; set; }
        public Game(string bindingsFile = null)
        {
            UIController.Init(this);
            PhaseManager = new(Unit.UnitAffiliation.Ally);
            mRenderer = new();
            CurrentMapIndex = 0;
            mKeyBindingsFile = bindingsFile;
            mRegistry = new Registry();
            if (mKeyBindingsFile != null)
            {
                if (File.Exists(mKeyBindingsFile))
                {
                    InputManager.ReadBindings(mKeyBindingsFile);
                }
            }
        }
        ~Game()
        {
            if (mKeyBindingsFile != null)
            {
                InputManager.WriteBindings(mKeyBindingsFile);
            }
        }
        /// <summary>
        /// The game's <see cref="FEEngine.Registry"/>
        /// </summary>
        public Registry Registry
        {
            get
            {
                return mRegistry;
            }
        }
        /// <summary>
        /// The game's <see cref="FEEngine.Renderer"/>
        /// </summary>
        public Renderer Renderer
        {
            get
            {
                return mRenderer;
            }
        }
        /// <summary>
        /// Starts the game loop
        /// </summary>
        /// <param name="player">The <see cref="Player"/> object</param>
        public void Loop(Player player)
        {
            PhaseManager.CyclePhase(Registry.GetRegister<Map>()[CurrentMapIndex]);
            while (true)
            {
                Update(player);
                if (InputManager.GetState().Quit)
                {
                    break;
                }
                Render();
            }
        }
        private void Update(Player player)
        {
            InputManager.Update();
            if (UIController.IsUnitContextMenuOpen)
            {
                UIController.FindMenu<UnitContextMenu>().Update();
            }
            else
            {
                player.Update();
            }
            Map map = mRegistry.GetRegister<Map>()[CurrentMapIndex];
            map.Update(PhaseManager.CurrentPhase);
            if (ShouldCyclePhase(map))
            {
                PhaseManager.CyclePhase(map);
            }
        }
        private bool ShouldCyclePhase(Map map)
        {
            bool shouldCycle = true;
            List<Unit> units = map.GetAllUnitsOfAffiliation(PhaseManager.CurrentPhase);
            foreach (Unit unit in units)
            {
                if (unit.CanMove)
                {
                    shouldCycle = false;
                    break;
                }
            }
            return shouldCycle;
        }
        private void Render()
        {
            Logger.PurgeLog();
            Renderer.ClearBuffer();
            Renderer.Render();
        }
        /// <summary>
        /// Initializes the standard registers
        /// </summary>
        public void SetupRegisters()
        {
            mRegistry.CreateRegister<Map>();
            mRegistry.CreateRegister<Unit>();
            mRegistry.CreateRegister<Item>();
            mRegistry.CreateRegister<Tile>();
            mRegistry.CreateRegister<Battalion>();
        }
        private readonly Registry mRegistry;
        private readonly string mKeyBindingsFile;
        private readonly Renderer mRenderer;
        private static bool debug = false;
        private static bool hasNativeImplementation = true;
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void BreakDebugger_Native();
    }
}
