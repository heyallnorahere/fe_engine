using System;
using System.Collections.Generic;
using FEEngine.Math;
using FEEngine.Menus;

namespace FEEngine
{
    public delegate void UpdateMethod();
    /// <summary>
    /// An object that controls the <see cref="Game"/>
    /// </summary>
    public class Player
    {
        public Player(Game game)
        {
            mUpdateHooks = new HashSet<UpdateMethod>();
            mLastColorFlip = 0.0;
            mRed = false;
            mSelectedIndex = -1;
            CursorPosition = new Vec2I(0);
            mGame = game;
        }
        /// <summary>
        /// The current cursor position on the <see cref="FEEngine.Map"/>
        /// </summary>
        public IVec2<int> CursorPosition { get; private set; }
        internal void Update()
        {
            foreach (UpdateMethod updateMethod in mUpdateHooks)
            {
                updateMethod();
            }
            Registry registry = mGame.Registry;
            if (registry.GetRegister<Map>().Count <= 0)
            {
                return;
            }
            if (Map.Player == null)
            {
                Map.Player = this;
            }
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
                if (delta.TaxicabLength() > 0)
                {
                    bool canMoveCursor = true;
                    Register<Unit> unitRegister = mGame.Registry.GetRegister<Unit>();
                    // check if the unit can move to the cursor's new position
                    if (mSelectedIndex != -1)
                    {
                        Unit unit = unitRegister[mSelectedIndex];
                        int futureLength = MathUtil.SubVectors(MathUtil.AddVectors(delta, CursorPosition), unit.Position).TaxicabLength();
                        canMoveCursor = futureLength <= unit.CurrentMovement;
                    }
                    // check if the currently selected unit can pass the next tile
                    if (canMoveCursor && mSelectedIndex != -1)
                    {
                        Tile? nextTile = Map.GetTileAt(MathUtil.AddVectors(delta, CursorPosition));
                        if (nextTile != null)
                        {
                            Unit unit = unitRegister[mSelectedIndex];
                            canMoveCursor = nextTile.CanPass(unit);
                        }
                    }
                    // check if there is an enemy unit at the cursor's new position
                    if (canMoveCursor && mSelectedIndex != -1)
                    {
                        Unit? candidateUnit = Map.GetUnitAt(MathUtil.AddVectors(delta, CursorPosition));
                        if (candidateUnit != null)
                        {
                            Unit unit = unitRegister[mSelectedIndex];
                            canMoveCursor = unit.IsAllied(candidateUnit);
                        }
                    }
                    if (canMoveCursor)
                    {
                        delta = MathUtil.SubVectors(MathUtil.ClampVector(MathUtil.AddVectors(delta, CursorPosition), new Vec2I(0), MathUtil.SubVectors(dimensions, new Vec2I(1))), CursorPosition);
                        IVec2<int> temp = CursorPosition;
                        MathUtil.AddVectors(ref temp, delta);
                        CursorPosition = temp;
                        this.VerifyValue(UIController.FindMenu<TileInfoMenu>()).SelectedTile = new Vec2I(CursorPosition);
                    }
                }
                if (state.OK)
                {
                    if (mSelectedIndex == -1)
                    {
                        Unit? unit = Map.GetUnitAt(CursorPosition);
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
                        this.VerifyValue(UIController.FindMenu<UnitContextMenu>()).OriginalUnitPosition = unit.Position;
                        unit.Move(CursorPosition);
                        mSelectedIndex = -1;
                        UIController.SelectedUnit = unit;
                        UIController.IsUnitContextMenuOpen = true;
                    }
                }
                if (state.Back && mSelectedIndex != -1)
                {
                    mSelectedIndex = -1; // just unselect the unit
                }
            }
        }
        internal void Render(RenderContext context)
        {
            if (CursorPosition.Y < Map.Height - 1 && !UIController.IsUnitContextMenuOpen)
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
                context.RenderChar(MathUtil.AddVectors(CursorPosition, new Vec2I(0, 1)), 'v', cursorColor);
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
        public bool AddUpdateHook(UpdateMethod update)
        {
            return mUpdateHooks.Add(update);
        }
        public bool RemoveUpdateHook(UpdateMethod update)
        {
            return mUpdateHooks.Remove(update);
        }
        private double mLastColorFlip;
        private bool mRed;
        private int mSelectedIndex;
        private readonly Game mGame;
        private readonly HashSet<UpdateMethod> mUpdateHooks;
    }
}
