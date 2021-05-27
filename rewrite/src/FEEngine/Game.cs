namespace FEEngine
{
    public class Game
    {
        public Game()
        {
            mRegistry = new Registry();
        }
        public Registry Registry
        {
            get
            {
                return mRegistry;
            }
        }
        public void SetupRegisters()
        {
            mRegistry.CreateRegister<Map>();
            mRegistry.CreateRegister<Unit>();
        }
        private Registry mRegistry;
    }
}
