using System;
using FEEngine.Math;

namespace FEEngine
{
    public class Player : IRenderable
    {
        public IVec2<int> CursorPosition { get; private set; }
        public PhaseManager PhaseManager { get; private set; }
        public Player(Game game, int mapRegisterIndex = 0)
        {
            PhaseManager = new PhaseManager();
            CursorPosition = new Vec2I(0);
            mGame = game;
            try
            {
                Register<Map> mapRegister = mGame.Registry.GetRegister<Map>();
                mMap = mapRegister[mapRegisterIndex];
            }
            catch (RegisterDoesNotExistException)
            {
                throw new Exception("The Game instance was not properly initialized");
            }
        }
        public void Update()
        {
            InputManager.State state = InputManager.GetState();
            IVec2<int> dimensions = mMap.Dimensions;
            CursorPosition = MathUtil.ClampVector(CursorPosition, new Vec2I(0), MathUtil.SubVectors(dimensions, new Vec2I(1)));
            if (PhaseManager.CurrentPhase == Unit.UnitAffiliation.Player)
            {
                IVec2<int> delta = new Vec2I(0);
                if (state.Up)
                {
                    MathUtil.AddVectors(ref delta, new Vec2I(0, 1));
                }
                if (state.Down)
                {
                    MathUtil.AddVectors(ref delta, new Vec2I(0, -1));
                }
                if (state.Left)
                {
                    MathUtil.AddVectors(ref delta, new Vec2I(-1, 0));
                }
                if (state.Right)
                {
                    MathUtil.AddVectors(ref delta, new Vec2I(1, 0));
                }
                delta = MathUtil.ClampVector(MathUtil.AddVectors(delta, CursorPosition), new Vec2I(0), MathUtil.SubVectors(dimensions, new Vec2I(0)));
                IVec2<int> temp = CursorPosition;
                MathUtil.AddVectors(ref temp, delta);
                CursorPosition = temp;
            }
        }
        public void Render(Renderer.Context context)
        {
            // todo: render
        }
        private Map mMap;
        private Game mGame;
    }
}
