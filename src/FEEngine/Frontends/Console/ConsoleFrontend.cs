using System;

namespace FEEngine.Frontends.Console
{
    [RegisteredFrontend("Console")]
    internal class ConsoleFrontend : Frontend
    {
        public override InputManager CreateInputManager()
        {
            mInputManager = new ConsoleInputManager();
            return mInputManager;
        }
        public override Renderer CreateRenderer()
        {
            throw new NotImplementedException();
        }
        public override void Run(Player player)
        {
            while (true)
            {
                mUpdateCallback?.Invoke(player);
                if (mInputManager?.GetState()?.Quit ?? false)
                {
                    break;
                }
                mRenderCallback?.Invoke();
            }
        }
        public override void SetRenderCallback(Action render)
        {
            mRenderCallback = render;
        }
        public override void SetUpdateCallback(Action<Player> update)
        {
            mUpdateCallback = update;
        }
        private Action? mRenderCallback;
        private Action<Player>? mUpdateCallback;
        private ConsoleInputManager? mInputManager;
    }
}
