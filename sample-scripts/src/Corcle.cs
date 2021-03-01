using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FEEngine;
using FEEngine.Math;

namespace Scripts {
    public class Corcle : Behavior
    {
        private List<Vec2<int>> vectors = new List<Vec2<int>>();
        private int frame = 0;
        public void OnAttach() {
            vectors.Add(new Vec2<int>(1, 0));
            vectors.Add(new Vec2<int>(0, 1));
            vectors.Add(new Vec2<int>(-1, 0));
            vectors.Add(new Vec2<int>(0, -1));
        }
        public void OnUpdate(InputMapper inputMapper)
        {
            this.Parent.Move(vectors[frame++]);
            frame %= vectors.Count;
        }
    }
}