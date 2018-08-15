using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    partial class Terrain
    {
        /// <summary>
        /// Represents one tile of a <see cref="Terrain"/>.
        /// </summary>
        public struct Point : IEquatable<Point>
        {
            /// <summary>
            /// The <see cref="Terrain"/> object.
            /// </summary>
            public Terrain Terrain => _terrain;
            private readonly Terrain _terrain;

            /// <summary>
            /// The <see cref="CivModel.Position"/> where this tile is.
            /// </summary>
            public Position Position
            {
                get => _position;
                set => _position = new Position { X = modulo(value.X, Terrain.Width), Y = value.Y };
            }
            private Position _position;

            /// <summary>
            /// The index of this tile,
            /// which is equal to <c><see cref="Position.Y"/> * <see cref="Width"/> + <see cref="Position.X"/></c>.
            /// </summary>
            /// <seealso cref="GetPoint(int)"/>
            public int Index => Position.Y * Terrain.Width + Position.X;

            private static int modulo(int a, int b)
            {
                if (b < 0)
                    b = -b;
                int r = a % b;
                if (r < 0)
                    r += b;
                return r;
            }

            /// <summary>
            /// <see cref="TerrainType"/> of the tile.
            /// </summary>
            public TerrainType Type
            {
                get => Terrain._points[Position.Y * Terrain.Width + Position.X].Type;
                set => Terrain._points[Position.Y * Terrain.Width + Position.X].Type = value;
            }

            /// <summary>
            /// <see cref="Player"/> which owns this tile. If no one owns this tile, <c>null</c>.
            /// </summary>
            /// <remarks>
            /// The setter of this property is wrapper of <see cref="Player.TryAddTerritory(Point)"/> and <see cref="Player.RemoveTerritory(Point)"/>.
            /// See these methods for more details and throwable exceptions.
            /// </remarks>
            /// <seealso cref="Player.TryAddTerritory(Point)"/>
            /// <seealso cref="Player.RemoveTerritory(Point)"/>
            public Player TileOwner
            {
                get => Terrain._points[Position.Y * Terrain.Width + Position.X].TileOwner;
                set
                {
                    if (value != TileOwner)
                    {
                        if (TileOwner != null)
                            TileOwner.RemoveTerritory(this);
                        if (value != null)
                            value.TryAddTerritory(this);
                    }
                }
            }

            /// <summary>
            /// The <see cref="Unit"/> placed at the tile.
            /// </summary>
            public Unit Unit => (Unit)GetTileObject(TileTag.Unit);

            /// <summary>
            /// The <see cref="TileBuilding"/> placed at the tile.
            /// </summary>
            public TileBuilding TileBuilding => (TileBuilding)GetTileObject(TileTag.TileBuilding);

            /// <summary>
            /// Initializes a new instance of the <see cref="Point"/> struct.
            /// </summary>
            /// <param name="terrain">The terrain object.</param>
            /// <param name="pos">The position where a tile will be.</param>
            /// <exception cref="ArgumentException"><paramref name="pos"/> is invalid.</exception>
            public Point(Terrain terrain, Position pos)
            {
                if (!terrain.IsValidPosition(pos))
                    throw new ArgumentException("pos", "pos is invalid");

                _terrain = terrain;

                _position = default(Position);
                Position = pos;
            }

            /// <summary>
            /// Get the distance between two points in the round Earth.
            /// </summary>
            /// <remarks>
            /// This method calculates the distance in the round Earth.
            /// Use <see cref="Position.Distance(Position, Position)"/> to calculate the distance in a flat space.
            /// </remarks>
            /// <param name="lhs">left hand side parameter</param>
            /// <param name="rhs">right hand side parameter</param>
            /// <exception cref="ArgumentException">points are on different terrains</exception>
            /// <returns>The distance between two <see cref="Position"/>.</returns>
            public static int Distance(Point lhs, Point rhs)
            {
                if (lhs.Terrain != rhs.Terrain)
                    throw new ArgumentException("points are on different terrains");

                Position p1 = lhs.Position;
                Position p2 = rhs.Position;
                Position p3 = lhs.Position;
                if (p2.X < p3.X)
                    p3.X -= lhs.Terrain.Width;
                else
                    p3.X += lhs.Terrain.Width;
                return Math.Min((p1 - p2).Norm(), (p2 - p3).Norm());
            }

            // this function is used internally by Terrain class and getters of this class.
            internal TileObject GetTileObject(TileTag tag)
            {
                return Terrain._points[Position.Y * Terrain.Width + Position.X].PlacedObjects[(int)tag];
            }

            // this function is used internally by Terrain class.
            internal void SetTileObject(TileObject obj)
            {
                Terrain._points[Position.Y * Terrain.Width + Position.X].PlacedObjects[(int)obj.TileTag] = obj;
            }

            // this function is used internally by Terrain class.
            internal void UnsetTileObject(TileTag tag)
            {
                Terrain._points[Position.Y * Terrain.Width + Position.X].PlacedObjects[(int)tag] = null;
            }

            // this function is used internally by Player class.
            internal void SetTileOwner(Player player)
            {
                Terrain._points[Position.Y * Terrain.Width + Position.X].TileOwner = player;
            }

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String" /> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                return Position.ToString();
            }

            /// <summary>
            /// Implements the operator ==.
            /// </summary>
            /// <param name="lhs">The LHS.</param>
            /// <param name="rhs">The RHS.</param>
            /// <returns>
            /// The result of the operator.
            /// </returns>
            public static bool operator ==(Point lhs, Point rhs)
            {
                return lhs.Terrain == rhs.Terrain && lhs.Position == rhs.Position;
            }
            /// <summary>
            /// Implements the operator !=.
            /// </summary>
            /// <param name="lhs">The LHS.</param>
            /// <param name="rhs">The RHS.</param>
            /// <returns>
            /// The result of the operator.
            /// </returns>
            public static bool operator !=(Point lhs, Point rhs)
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
                return obj is Point pt && Equals(pt);
            }
            /// <summary>
            /// 현재 개체가 동일한 형식의 다른 개체와 같은지 여부를 나타냅니다.
            /// </summary>
            /// <param name="other">이 개체와 비교할 개체입니다.</param>
            /// <returns>
            /// 현재 개체가 <see langword="true" /> 매개 변수와 같으면 <paramref name="other" />이고, 그렇지 않으면 <see langword="false" />입니다.
            /// </returns>
            public bool Equals(Point other)
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
                var hashCode = -715914744;
                hashCode = hashCode * -1521134295 + Terrain.GetHashCode();
                hashCode = hashCode * -1521134295 + Position.GetHashCode();
                return hashCode;
            }

            /// <summary>
            /// Gets the list of points, within the specified distance, in left-to-right, top-to-bottom order.
            /// </summary>
            /// <param name="distance">The distance</param>
            /// <returns>The list of points</returns>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="distance"/> is negative</exception>
            /// <seealso cref="AdjacentsAtDistance(int)"/>
            /// <seealso cref="Position.AdjacentsWithinDistance(int)"/>
            public IEnumerable<Point?> AdjacentsWithinDistance(int distance)
            {
                return Position.AdjacentsWithinDistance(distance).Select(Terrain.TryGetPoint);
            }

            /// <summary>
            /// Gets the list of points, at the specified distance, in clockwise order.
            /// </summary>
            /// <param name="distance">The distance</param>
            /// <returns>The list of points</returns>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="distance"/> is negative</exception>
            /// <seealso cref="AdjacentsWithinDistance(int)"/>
            /// <seealso cref="Position.AdjacentsAtDistance(int)"/>
            public IEnumerable<Point?> AdjacentsAtDistance(int distance)
            {
                return Position.AdjacentsAtDistance(distance).Select(Terrain.TryGetPoint);
            }

            /// <summary>
            /// Gets the list of adjacent points, in clockwise order.
            /// </summary>
            /// <returns>The list of points</returns>
            /// <seealso cref="AdjacentsWithinDistance(int)"/>
            /// <seealso cref="AdjacentsAtDistance(int)"/>
            /// <seealso cref="Position.Adjacents"/>
            public Point?[] Adjacents()
            {
                return AdjacentsAtDistance(1).ToArray();
            }
        }
    }
}
