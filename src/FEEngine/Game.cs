using System;
using System.Collections.Generic;
using System.IO;
using FEEngine.Menus;

namespace FEEngine
{
    /// <summary>
    /// An object that oversees the entire game's state
    /// </summary>
    public sealed class Game
    {
        /// <summary>
        /// The game's <see cref="FEEngine.PhaseManager"/>
        /// </summary>
        public PhaseManager PhaseManager { get; private set; }
        /// <summary>
        /// The index of the current <see cref="Map"/> that is being played
        /// </summary>
        public int CurrentMapIndex { get; set; }
        public Game(string? bindingsFile = null)
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
            SetupRegisters();
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
            Register<Map> mapRegister = Registry.GetRegister<Map>();
            if (mapRegister.Count > 0)
            {
                PhaseManager.CyclePhase(mapRegister[CurrentMapIndex]);
            }
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
                UIController.FindMenu<UnitContextMenu>()?.Update();
            }
            else
            {
                player.Update();
            }
            Register<Map> mapRegister = mRegistry.GetRegister<Map>();
            if (mapRegister.Count > 0)
            {
                Map map = mapRegister[CurrentMapIndex];
                map.Update(PhaseManager.CurrentPhase);
                if (ShouldCyclePhase(map))
                {
                    PhaseManager.CyclePhase(map);
                }
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
        private void SetupRegisters()
        {
            mRegistry.CreateRegister<Map>();
            mRegistry.CreateRegister<Unit>();
            mRegistry.CreateRegister<Item>();
            mRegistry.CreateRegister<Tile>();
            mRegistry.CreateRegister<Battalion>();
        }
        private readonly Registry mRegistry;
        private readonly string? mKeyBindingsFile;
        private readonly Renderer mRenderer;
    }
}
