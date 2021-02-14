using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace FEEngine.Math
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vec2
    {
        public int X, Y;
        public Vec2(int scalar)
        {
            this.X = this.Y = scalar;
        }
        public Vec2(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public void Clamp(Vec2 min, Vec2 max)
        {
            if (this.X < min.X)
            {
                this.X = min.X;
            }
            if (this.X > max.X)
            {
                this.X = max.X;
            }
            if (this.Y < min.Y)
            {
                this.Y = min.Y;
            }
            if (this.Y > max.Y)
            {
                this.Y = max.Y;
            }
        }
        public static Vec2 operator-(Vec2 vector)
        {
            return new Vec2(-vector.X, -vector.Y);
        }
    }
}
