namespace FEEngine
{
    public class Game
    {
        public static bool Debug
        {
            get => debug;
            set => debug = value;
        }
        public Game(string bindingsFile = null)
        {
            mKeyBindingsFile = bindingsFile;
            mRegistry = new Registry();
            InputManager.Init();
            if (mKeyBindingsFile != null)
            {
                InputManager.ReadBindings(mKeyBindingsFile);
            }
        }
        ~Game()
        {
            if (mKeyBindingsFile != null)
            {
                InputManager.WriteBindings(mKeyBindingsFile);
            }
        }
        public Registry Registry
        {
            get
            {
                return mRegistry;
            }
        }
        public void Loop(Player player)
        {
            while (true)
            {
                Update(player);
                if (InputManager.GetState().Quit)
                {
                    break;
                }
            }
        }
        private void Update(Player player)
        {
            InputManager.Update();
            player.Update();
        }
        public void SetupRegisters()
        {
            mRegistry.CreateRegister<Map>();
            mRegistry.CreateRegister<Unit>();
            mRegistry.CreateRegister<Item>();
        }
        private Registry mRegistry;
        private string mKeyBindingsFile;
        private static bool debug = false;
    }
}
