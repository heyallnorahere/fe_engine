using System;
using FEEngine.Math;

namespace FEEngine
{
    public class Player
    {
        public IVec2<int> CursorPosition { get; private set; }
        public PhaseManager PlayerPhaseManager { get; private set; }
        public Player(Game game, int mapRegisterIndex = 0)
        {
            PlayerPhaseManager = new PhaseManager();
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
            IVec2<int> dimensions = mMap.Dimensions;
            CursorPosition = MathUtil.ClampVector(CursorPosition, new Vec2I(0), new Vec2I(dimensions.X - 1, dimensions.Y - 1));
            if (PlayerPhaseManager.CurrentPhase == Unit.UnitAffiliation.Player)
            {
                IVec2<int> delta = new Vec2I(0);
                InputManager.State state = InputManager.GetState();
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
                if (delta.TaxicabLength() > 0)
                {
                    Console.WriteLine("Moving: ({0}, {1})", delta.X, delta.Y);
                }
            }
        }
        private Map mMap;
        private Game mGame;
    }
}
