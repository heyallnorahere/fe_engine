using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using FEEngine.Math;

namespace FEEngine
{
    [JsonObject]
    public class Map : RegistedObjectTemplate<Map>, IEnumerable<Unit>, IRenderable
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
        public int Width { get; set; }
        public int Height { get; set; }
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
        public List<int> Units { get; set; }
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
        public override void OnDeserialization()
        {
            foreach (Unit unit in this)
            {
                unit.Parent = this;
            }
        }
        public void Update(Unit.UnitAffiliation currentPhase)
        {
            List<Unit> units = GetAllUnitsOfAffiliation(currentPhase);
            foreach (Unit unit in units)
            {
                if (unit.CanMove)
                {
                    unit.Update();
                }
            }
        }
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
        public void Render()
        {
            int yOffset = Renderer.BufferSize.Y - Height;
            foreach (Unit unit in this)
            {
                // todo: replace 'U' with character corresponding to the units weapon type
                Renderer.RenderChar(MathUtil.AddVectors(unit.Position, new Vec2I(0, yOffset)), 'U', Unit.GetColorForAffiliation(unit.Affiliation));
            }
        }
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
        [JsonConstructor]
        public Map(int width, int height, List<int> units = null)
        {
            Width = width;
            Height = height;
            Units = units ?? new List<int>();
        }
    }
}
