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
        public struct Point
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
                if (obj is Point other)
                    return this == other;
                return false;
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    return Terrain.GetHashCode() * 17 + Position.GetHashCode();
                }
            }

            /// <summary>
            /// get adjacent points, in clockwise order.
            /// </summary>
            /// <remarks>
            /// Get the array of the adjacent points in clockwise order.
            /// If the position is invalid, the value is null.
            /// A first element of the array is the left one.
            ///   1   2
            /// 0  pt  3
            ///   5   4
            /// </remarks>
            /// <returns>an array of the adjacent points</returns>
            public Point?[] Adjacents()
            {
                var ret = new Point?[6];
                Position pos;

                pos = new Position { X = Position.X - 1, Y = Position.Y };
                if (Terrain.IsValidPosition(pos))
                    ret[0] = new Point(Terrain, pos);

                pos = new Position { X = Position.X, Y = Position.Y - 1 };
                if (Terrain.IsValidPosition(pos))
                    ret[2] = ret[1] = new Point(Terrain, pos);

                pos = new Position { X = Position.X + 1, Y = Position.Y };
                if (Terrain.IsValidPosition(pos))
                    ret[3] = new Point(Terrain, pos);

                pos = new Position { X = Position.X, Y = Position.Y + 1 };
                if (Terrain.IsValidPosition(pos))
                    ret[5] = ret[4] = new Point(Terrain, pos);

                int correction = 1 - (Position.Y % 2) * 2;
                int cidx = 1 - (Position.Y % 2);

                if (ret[1 + cidx] != null)
                {
                    pos = ret[1 + cidx].Value.Position;
                    pos.X += correction;
                    ret[1 + cidx] = new Point(Terrain, pos);
                }
                if (ret[5 - cidx] != null)
                {
                    pos = ret[5 - cidx].Value.Position;
                    pos.X += correction;
                    ret[5 - cidx] = new Point(Terrain, pos);
                }

                return ret;
            }
        }
    }
}
