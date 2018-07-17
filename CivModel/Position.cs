using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// Represents the coordinate for <see cref="Terrain"/>.
    /// The coordinate system is documented in "docs/Coordinate System.pptx".
    /// </summary>
    public struct Position : IEquatable<Position>
    {
        /// <summary>
        /// X in physical coordinate system.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Y in physical coordinate system.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// A in logical coordinate system.
        /// </summary>
        public int A => -X - ((Y + (Math.Sign(Y) - 1) / 2) / 2);

        /// <summary>
        /// B in logical coordinate system.
        /// </summary>
        public int B => -A - C;

        /// <summary>
        /// C in logical coordinate system.
        /// </summary>
        public int C => Y;

        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> structure from the physical coordinates.
        /// </summary>
        /// <param name="x">X in physical coordinate system.</param>
        /// <param name="y">Y in physical coordinate system.</param>
        /// <returns>the created <see cref="Position"/></returns>
        public static Position FromPhysical(int x, int y)
        {
            return new Position { X = x, Y = y };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> structure from the logical coordinates.
        /// </summary>
        /// <param name="a">A in logical coordinate system.</param>
        /// <param name="b">B in logical coordinate system.</param>
        /// <param name="c">C in logical coordinate system.</param>
        /// <returns>the created <see cref="Position"/></returns>
        /// <exception cref="ArgumentException">logical coordinate is invalid</exception>
        public static Position FromLogical(int a, int b, int c)
        {
            if (a + b + c != 0)
                throw new ArgumentException("logical coordinate is invalid");

            return new Position {
                X = -a - ((c + (Math.Sign(c) - 1) / 2) / 2),
                Y = c
            };
        }

        /// <summary>
        /// Gets the list of positions, within the specified distance, in left-to-right, top-to-bottom order.
        /// </summary>
        /// <param name="distance">The distance</param>
        /// <returns>The list of positions</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="distance"/> is negative</exception>
        /// <seealso cref="AdjacentsAtDistance(int)"/>
        /// <seealso cref="Terrain.Point.AdjacentsWithinDistance(int)"/>
        public IEnumerable<Position> AdjacentsWithinDistance(int distance)
        {
            if (distance < 0)
                throw new ArgumentOutOfRangeException(nameof(distance), "distance is negative");

            for (int dc = -distance; dc <= distance; ++dc)
            {
                int da0 = Math.Max(0, -dc) - distance;
                for (int da = distance - Math.Max(0, dc); da >= da0; --da)
                {
                    int db = 0 - dc - da;
                    yield return FromLogical(A + da, B + db, C + dc);
                }
            }
        }

        /// <summary>
        /// Gets the list of positions, at the specified distance, in clockwise order.
        /// </summary>
        /// <param name="distance">The distance</param>
        /// <returns>The list of positions</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="distance"/> is negative</exception>
        /// <remarks>
        /// Get the list of the adjacent points in clockwise order.
        /// A first element of the list is the left one.
        /// <code>
        /// // the result when distance = 1
        /// //   1   2
        /// // 0  pt  3
        /// //   5   4
        /// </code>
        /// </remarks>
        /// <seealso cref="AdjacentsWithinDistance(int)"/>
        /// <seealso cref="Terrain.Point.AdjacentsAtDistance(int)"/>
        public IEnumerable<Position> AdjacentsAtDistance(int distance)
        {
            if (distance < 0)
                throw new ArgumentOutOfRangeException(nameof(distance), "distance is negative");

            int da = distance;
            int db = -distance;
            int dc = 0;

            for (; db < 0; ++db, --dc)
                yield return FromLogical(A + da, B + db, C + dc);

            for (; da > 0; --da, ++db)
                yield return FromLogical(A + da, B + db, C + dc);

            for (; dc < 0; ++dc, --da)
                yield return FromLogical(A + da, B + db, C + dc);

            for (; db > 0; --db, ++dc)
                yield return FromLogical(A + da, B + db, C + dc);

            for (; da < 0; ++da, --db)
                yield return FromLogical(A + da, B + db, C + dc);

            for (; dc > 0; --dc, ++da)
                yield return FromLogical(A + da, B + db, C + dc);
        }

        /// <summary>
        /// Gets the list of adjacent positions, in clockwise order.
        /// </summary>
        /// <returns>The list of positions</returns>
        /// <seealso cref="AdjacentsWithinDistance(int)"/>
        /// <seealso cref="AdjacentsAtDistance(int)"/>
        /// <seealso cref="Terrain.Point.Adjacents"/>
        public Position[] Adjacents()
        {
            return AdjacentsAtDistance(1).ToArray();
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Position operator +(Position obj)
        {
            return obj;
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Position operator -(Position obj)
        {
            return new Position { X = -obj.X, Y = -obj.Y };
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Position operator +(Position lhs, Position rhs)
        {
            return FromLogical(lhs.A + rhs.A, lhs.B + rhs.B, lhs.C + rhs.C);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Position operator -(Position lhs, Position rhs)
        {
            return FromLogical(lhs.A - rhs.A, lhs.B - rhs.B, lhs.C - rhs.C);
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Position operator *(int lhs, Position rhs)
        {
            return FromLogical(lhs * rhs.A, lhs * rhs.B, lhs * rhs.C);
        }

        /// <summary>
        /// Get norm of this position. It is equal to <c>(|A| + |B| + |C|) / 2</c>
        /// </summary>
        /// <returns>The norm of this position.</returns>
        public int Norm()
        {
            return (Math.Abs(A) + Math.Abs(B) + Math.Abs(C)) / 2;
        }

        /// <summary>
        /// Get the distance between two <see cref="Position"/> in a flat space.
        /// It is equal to <c>(<paramref name="lhs"/> - <paramref name="rhs"/>).<see cref="Norm"/>()</c>.
        /// </summary>
        /// <remarks>
        /// This method does not regard the round Earth.
        /// Use <see cref="Terrain.Point.Distance(Terrain.Point, Terrain.Point)"/> to calculate a distance in the round Earth.
        /// </remarks>
        /// <param name="lhs">left hand side parameter</param>
        /// <param name="rhs">right hand side parameter</param>
        /// <returns>The distance between two <see cref="Position"/>.</returns>
        public static int Distance(Position lhs, Position rhs)
        {
            return (lhs - rhs).Norm();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("[{0},{1}] ({2},{3},{4})", X, Y, A, B, C);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Position lhs, Position rhs)
        {
            return lhs.X == rhs.X && lhs.Y == rhs.Y;
        }
        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Position lhs, Position rhs)
        {
            return !(lhs == rhs);
        }
        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is Position pos && Equals(pos);
        }
        /// <summary>
        /// 현재 개체가 동일한 형식의 다른 개체와 같은지 여부를 나타냅니다.
        /// </summary>
        /// <param name="other">이 개체와 비교할 개체입니다.</param>
        /// <returns>
        /// 현재 개체가 <see langword="true" /> 매개 변수와 같으면 <paramref name="other" />이고, 그렇지 않으면 <see langword="false" />입니다.
        /// </returns>
        public bool Equals(Position other)
        {
            return (this == other);
        }
        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }
    }
}
