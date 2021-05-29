using System;
using FEEngine.Math;

namespace FEEngine
{
    public class Player : IRenderable
    {
        public IVec2<int> CursorPosition { get; private set; }
        public Player(Game game)
        {
            mLastColorFlip = 0.0;
            mRed = false;
            mSelectedIndex = -1;
            CursorPosition = new Vec2I(0);
            mGame = game;
        }
        public void Update()
        {
            InputManager.State state = InputManager.GetState();
            IVec2<int> dimensions = Map.Dimensions;
            CursorPosition = MathUtil.ClampVector(CursorPosition, new Vec2I(0), MathUtil.SubVectors(dimensions, new Vec2I(1)));
            if (mGame.PhaseManager.CurrentPhase == Unit.UnitAffiliation.Player)
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
                bool canMoveCursor = true;
                if (mSelectedIndex != -1)
                {
                    Register<Unit> unitRegister = mGame.Registry.GetRegister<Unit>();
                    Unit unit = unitRegister[mSelectedIndex];
                    int futureLength = MathUtil.AddVectors(delta, CursorPosition).TaxicabLength();
                    canMoveCursor = futureLength <= unit.CurrentMovement;
                }
                if (canMoveCursor)
                {
                    delta = MathUtil.SubVectors(MathUtil.ClampVector(MathUtil.AddVectors(delta, CursorPosition), new Vec2I(0), MathUtil.SubVectors(dimensions, new Vec2I(0))), CursorPosition);
                    IVec2<int> temp = CursorPosition;
                    MathUtil.AddVectors(ref temp, delta);
                    CursorPosition = temp;
                }
                if (state.OK)
                {
                    if (mSelectedIndex == -1)
                    {
                        Unit unit = Map.GetUnitAt(CursorPosition);
                        if (unit != null)
                        {
                            if (unit.CanMove)
                            {
                                mSelectedIndex = unit.RegisterIndex;
                            }
                        }
                    }
                    else
                    {
                        Register<Unit> unitRegister = mGame.Registry.GetRegister<Unit>();
                        Unit unit = unitRegister[mSelectedIndex];
                        unit.Move(CursorPosition);
                        mSelectedIndex = -1;
                        // todo: hand unit over to UIController, but until thats implemented, wait
                        unit.Wait();
                    }
                }
            }
        }
        public void Render(Renderer.Context context)
        {
            if (CursorPosition.Y < Map.Height - 1)
            {
                Color cursorColor = Color.White;
                if (mSelectedIndex != -1)
                {
                    double now = DateTime.Now.TimeOfDay.TotalSeconds;
                    const double interval = 0.5;
                    if (now - mLastColorFlip > interval)
                    {
                        mLastColorFlip = now;
                        mRed = !mRed;
                    }
                    cursorColor = mRed ? Color.Red : Color.Black;
                }
                int yOffset = context.BufferSize.Y - Map.Height;
                Renderer.RenderChar(MathUtil.AddVectors(CursorPosition, new Vec2I(0, yOffset + 1)), 'v', cursorColor);
            }
        }
        private Map Map
        {
            get
            {
                Register<Map> mapRegister = mGame.Registry.GetRegister<Map>();
                return mapRegister[mGame.CurrentMapIndex];
            }
        }
        private double mLastColorFlip;
        private bool mRed;
        private int mSelectedIndex;
        private readonly Game mGame;
    }
}
