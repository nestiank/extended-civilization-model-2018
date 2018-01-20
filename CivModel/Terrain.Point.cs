using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    public partial class Terrain
    {
        public struct Point
        {
            public readonly Terrain _terrain;
            public Terrain Terrain => _terrain;

            public Position Position { get; private set; }

            public TerrainType1 Type1 => Terrain._points[Position.X, Position.Y].Type1;
            public TerrainType2 Type2 => Terrain._points[Position.X, Position.Y].Type2;
            public Unit Unit => (Unit)Terrain._points[Position.X, Position.Y].PlacedObjects[(int)TileTag.Unit];
            public TileBuilding TileBuilding => (TileBuilding)Terrain._points[Position.X, Position.Y].PlacedObjects[(int)TileTag.TileBuilding];

            public Point(Terrain terrain, Position pos)
            {
                if (!terrain.IsValidPosition(pos))
                    throw new ArgumentOutOfRangeException();

                _terrain = terrain;
                Position = pos;
            }

            public override string ToString()
            {
                return Position.ToString();
            }

            public static bool operator ==(Point lhs, Point rhs)
            {
                return lhs.Terrain == rhs.Terrain && lhs.Position == rhs.Position;
            }
            public static bool operator !=(Point lhs, Point rhs)
            {
                return !(lhs == rhs);
            }
            public override bool Equals(object obj)
            {
                if (obj is Point other)
                    return this == other;
                return false;
            }
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
