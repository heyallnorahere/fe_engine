using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using FEEngine.Math;

namespace FEEngine
{
    public class Tile
    {
        public struct PassingPropertiesStruct
        {
            public bool Foot;
        }
        public Vec2<int> Position { get; private set; }
        public Map Parent
        {
            get
            {
                return Map.MakeFromRegisterIndex(this.parentIndex);
            }
        }
        private ulong parentIndex;
        public PassingPropertiesStruct PassingProperties
        {
            get
            {
                return GetPassingProperties_Native(this.parentIndex, this.Position);
            }
        }
        public Renderer.Color Color
        {
            get
            {
                return GetColor_Native(this.parentIndex, this.Position);
            }
        }
        private Tile()
        {
            this.Position = new Vec2<int>(0, 0);
            this.parentIndex = 0;
        }
        public static Tile MakeFromPosition(Vec2<int> position, Map map)
        {
            Tile tile = new Tile();
            tile.Position = position;
            tile.parentIndex = map.Index;
            return tile;
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern PassingPropertiesStruct GetPassingProperties_Native(ulong mapIndex, Vec2<int> tilePosition);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Renderer.Color GetColor_Native(ulong mapIndex, Vec2<int> tilePosition);
    }
}
