using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    public struct Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int A => -B - C;
        public int B => X - (Y + Math.Sign(Y)) / 2;
        public int C => Y;

        public static Position FromPhysical(int x, int y)
        {
            return new Position { X = x, Y = y };
        }

        public static Position FromLogical(int a, int b, int c)
        {
            if (a + b + c != 0)
                throw new ArgumentException("logical coordinate is invalid");

            return new Position {
                X = b + (c + Math.Sign(c)) / 2,
                Y = c
            };
        }

        public static Position operator +(Position obj)
        {
            return obj;
        }
        public static Position operator -(Position obj)
        {
            return new Position { X = -obj.X, Y = -obj.Y };
        }
        public static Position operator +(Position lhs, Position rhs)
        {
            return FromLogical(lhs.A + rhs.A, lhs.B + rhs.B, lhs.C + rhs.C);
        }
        public static Position operator -(Position lhs, Position rhs)
        {
            return FromLogical(lhs.A - rhs.A, lhs.B - rhs.B, lhs.C - rhs.C);
        }
        public static Position operator *(int lhs, Position rhs)
        {
            return FromLogical(lhs * rhs.A, lhs * rhs.B, lhs * rhs.C);
        }

        public int Norm()
        {
            return (Math.Abs(A) + Math.Abs(B) + Math.Abs(C)) / 2;
        }
        public static int Distance(Position lhs, Position rhs)
        {
            return (lhs - rhs).Norm();
        }

        public override string ToString()
        {
            return string.Format("[{0},{1}] ({2},{3},{4})", X, Y, A, B, C);
        }

        public static bool operator ==(Position lhs, Position rhs)
        {
            return lhs.X == rhs.X && lhs.Y == rhs.Y;
        }
        public static bool operator !=(Position lhs, Position rhs)
        {
            return !(lhs == rhs);
        }
        public override bool Equals(object obj)
        {
            if (obj is Position other)
                return this == other;
            return false;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return X * 17 + Y;
            }
        }
    }
}
