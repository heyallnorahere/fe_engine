using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using FEEngine.Math;

namespace FEEngine
{
    [JsonObject]
    public class Map : RegistedObjectTemplate<Map>, IEnumerable
    {
        private struct Enumerator : IEnumerator
        {
            public bool MoveNext()
            {
                return false;
            }
            public void Reset()
            {
                //
            }
            public object Current
            {
                get
                {
                    Registry registry = mParent.mRegister.Parent;
                    Register<Unit> unitRegister = registry.GetRegister<Unit>();
                    return unitRegister[mParent.Units[mPosition]];
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
        public List<int> Units { get; private set; }
        public void AddUnit(Unit unit)
        {
            unit.Parent = this;
            Units.Add(unit.RegisterIndex);
        }
        public IEnumerator GetEnumerator()
        {
            return new Enumerator(this);
        }
        [JsonConstructor]
        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            Units = new List<int>();
        }
    }
}
