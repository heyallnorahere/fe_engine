/*
   Copyright 2022 Nora Beda

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Diagnostics.CodeAnalysis;

namespace FEEngine
{
    public struct Vector
    {
        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Vector(int scalar)
        {
            X = Y = scalar;
        }

        public int X, Y;
        public int TaxicabLength => Math.Abs(X) + Math.Abs(Y);

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is Vector vector)
            {
                return X == vector.X && Y == vector.Y;
            }

            return false;
        }

        public override int GetHashCode() => HashCode.Combine(X, Y);

        public static implicit operator Vector((int x, int y) t) => new(t.x, t.y);

        public static bool operator ==(Vector lhs, Vector rhs) => lhs.Equals(rhs);
        public static bool operator !=(Vector lhs, Vector rhs) => !lhs.Equals(rhs);

        public static Vector operator +(Vector lhs, Vector rhs) => (lhs.X + rhs.X, lhs.Y + rhs.Y);
        public static Vector operator +(Vector lhs, int rhs) => lhs + (rhs, rhs);
        public static Vector operator +(int lhs, Vector rhs) => (lhs, lhs) + rhs;

        public static Vector operator -(Vector vector) => (-vector.X, -vector.Y);
        public static Vector operator -(Vector lhs, Vector rhs) => lhs + -rhs;
        public static Vector operator -(Vector lhs, int rhs) => lhs - (rhs, rhs);
        public static Vector operator -(int lhs, Vector rhs) => (lhs, lhs) - rhs;

        public override string ToString() => $"({X}, {Y})";
    }
}
