using System;
using System.Collections.Generic;

namespace CivModel
{
    public enum TerrainType1
    {
        Grass, Flatland, Swamp, Tundra
    }
    public enum TerrainType2
    {
        None, Hill, Mountain
    }

    public struct Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public override string ToString()
        {
            return string.Format("({0},{1})", X, Y);
        }
    }

    public class Terrain
    {
        private struct Point_t
        {
            public TerrainType1 Type1;
            public TerrainType2 Type2;
            public TileObject[] PlacedObjects;
        }
        public struct Point
        {
            public readonly Terrain _terrain;
            public Terrain Terrain => _terrain;

            public Position Position { get; private set; }

            public TerrainType1 Type1 => Terrain._points[Position.X, Position.Y].Type1;
            public TerrainType2 Type2 => Terrain._points[Position.X, Position.Y].Type2;
            public Unit PlacedUnit => (Unit)Terrain._points[Position.X, Position.Y].PlacedObjects[(int)TileTag.Unit];
            public District District => (District)Terrain._points[Position.X, Position.Y].PlacedObjects[(int)TileTag.District];

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
                    pos = ret[5 -cidx].Value.Position;
                    pos.X += correction;
                    ret[5 - cidx] = new Point(Terrain, pos);
                }

                return ret;
            }
        }

        private Point_t[,] _points;

        public readonly int _width, _height;
        public int Width => _width;
        public int Height => _height;

        public Terrain(int width, int height)
        {
            _width = width;
            _height = height;

            _points = new Point_t[height, width];

            var random = new Random();
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int len = Enum.GetNames(typeof(TileTag)).Length;
                    _points[y, x].PlacedObjects = new TileObject[len];

                    _points[y, x].Type1 = (TerrainType1)random.Next(4);
                    _points[y, x].Type2 = (TerrainType2)random.Next(3);
                }
            }
        }

        public Terrain.Point GetPoint(int x, int y)
        {
            return GetPoint(new Position { X = x, Y = y });
        }
        public Terrain.Point GetPoint(Position pos)
        {
            return new Terrain.Point(this, pos);
        }

        public void PlaceObject(TileObject obj)
        {
            var p = obj.PlacedPoint
                ?? throw new ArgumentException("obj.PlacedPoint is null");

            TileObject prev = _points[p.Position.X, p.Position.Y].PlacedObjects[(int)obj.TileTag];
            if (prev != null)
            {
                prev.PlacedPoint = null;
            }

            _points[p.Position.X, p.Position.Y].PlacedObjects[(int)obj.TileTag] = obj;
        }

        public void UnplaceObject(TileObject obj, Point p)
        {
            if (obj.PlacedPoint != null)
                throw new ArgumentException("obj.PlacedPoint is not null");
            if (p.Terrain != this)
                throw new ArgumentException("UnplacedUnit() call with wrong Terrain object");

            if (_points[p.Position.X, p.Position.Y].PlacedObjects[(int)obj.TileTag] != obj)
                throw new ArgumentException("obj is not on the specified point");

            _points[p.Position.X, p.Position.Y].PlacedObjects[(int)obj.TileTag] = null;
        }

        public bool IsValidPosition(Position pos)
        {
            if (pos.X < 0 || pos.X >= Width)
                return false;
            if (pos.Y < 0 || pos.Y >= Height)
                return false;
            return true;
        }
    }
}
