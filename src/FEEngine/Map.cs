using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using FEEngine.Math;

namespace FEEngine
{
    /// <summary>
    /// The object through which <see cref="Unit"/>s interact
    /// </summary>
    [JsonObject]
    public class Map : RegisteredObjectBase<Map>, IEnumerable<Unit>, IRenderable
    {
        private struct Enumerator : IEnumerator<Unit>
        {
            public bool MoveNext()
            {
                if (mPosition < mParent.Units.Count - 1)
                {
                    mPosition++;
                    return true;
                }
                return false;
            }
            public void Reset()
            {
                mPosition = -1;
            }
            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }
            public Unit Current
            {
                get
                {
                    Registry registry = mParent.mRegister.Parent;
                    Register<Unit> unitRegister = registry.GetRegister<Unit>();
                    return unitRegister[mParent.Units[mPosition]];
                }
            }
            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }
            public Enumerator(Map parent)
            {
                mPosition = -1;
                mParent = parent;
            }
            private int mPosition;
            private Map mParent;
        }
        /// <summary>
        /// The width of the <see cref="Map"/>
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// The height of the <see cref="Map"/>
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// The culmination <see cref="Width"/> and <see cref="Height"/>
        /// </summary>
        [JsonIgnore]
        public IVec2<int> Dimensions
        {
            get
            {
                return new Vec2I(Width, Height);
            }
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }
        /// <summary>
        /// A list of <see cref="Register{T}"/> indices that point to active <see cref="Unit"/>. DO NOT add units through this; use <see cref="AddUnit(Unit)"/>
        /// </summary>
        public List<int> Units { get; set; }
        [JsonIgnore]
        internal Player Player { get; set; }
        public IVec2<int> MinSize { get => Dimensions; }
        /// <summary>
        /// Finds a <see cref="Unit"/> at the specified position
        /// </summary>
        /// <param name="position">The position to search at</param>
        /// <returns>A <see cref="Unit"/> object if found; otherwise, null</returns>
        public Unit GetUnitAt(IVec2<int> position)
        {
            foreach (Unit unit in this)
            {
                if (MathUtil.AreVectorsEqual(unit.Position, position))
                {
                    return unit;
                }
            }
            return null;
        }
        public override void OnDeserialized()
        {
            foreach (Unit unit in this)
            {
                unit.Parent = this;
            }
        }
        internal void Update(Unit.UnitAffiliation currentPhase)
        {
            List<Unit> units = GetAllUnitsOfAffiliation(currentPhase);
            foreach (Unit unit in units)
            {
                if (unit.CanMove)
                {
                    unit.Update();
                }
            }
            // go again, to remove units with 0 hp
            bool keepGoing = true;
            while (keepGoing)
            {
                keepGoing = false;
                foreach (Unit unit in this)
                {
                    if (unit.CurrentHP <= 0)
                    {
                        keepGoing = true;
                        Units.Remove(unit.RegisterIndex);
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// Gets all <see cref="Unit"/>s on the map of the specified affiliation
        /// </summary>
        /// <param name="affiliation">See summary</param>
        /// <returns>See summary</returns>
        public List<Unit> GetAllUnitsOfAffiliation(Unit.UnitAffiliation affiliation)
        {
            List<Unit> units = new();
            foreach (Unit unit in this)
            {
                if (unit.Affiliation == affiliation)
                {
                    units.Add(unit);
                }
            }
            return units;
        }
        public void Render(RenderContext context)
        {
            if (mRenderSize.X < Width || mRenderSize.Y < Height)
            {
                return;
            }
            int xDifference = mRenderSize.X - Width;
            if (xDifference % 2 > 0)
            {
                xDifference--;
            }
            int padding = xDifference / 2;
            context.PushPair(new RenderContext.OffsetClipPair()
            {
                Offset = new Vec2I(padding, 0),
                Clip = MathUtil.SubVectors(mRenderSize, new Vec2I(padding, 0))
            });
            RenderMap(context);
            context.PopPair();
        }
        private void RenderMap(RenderContext context)
        {
            foreach (Unit unit in this)
            {
                // todo: replace 'U' with character corresponding to the units weapon type
                Item equippedWeapon = unit.EquippedWeapon;
                context.RenderChar(unit.Position, WeaponStats.GetCharacterForWeapon(equippedWeapon?.WeaponStats ?? null), Unit.GetColorForAffiliation(unit.Affiliation));
            }
            Player.Render(context);
        }
        /// <summary>
        /// Adds a <see cref="Unit"/> to the list of units
        /// </summary>
        /// <param name="unit">The <see cref="Unit"/> to add</param>
        public void AddUnit(Unit unit)
        {
            unit.Parent = this;
            Units.Add(unit.RegisterIndex);
        }
        public IEnumerator<Unit> GetEnumerator()
        {
            return new Enumerator(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public void SetSize(IVec2<int> size)
        {
            mRenderSize = size;
        }
        [JsonConstructor]
        public Map(int width, int height, List<int> units = null)
        {
            Width = width;
            Height = height;
            Units = units ?? new List<int>();
            Player = null;
        }
        private IVec2<int> mRenderSize;
    }
}
