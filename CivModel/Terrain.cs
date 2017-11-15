using System;

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
    }

    public class Terrain
    {
        private struct Point_t
        {
            public TerrainType1 Type1;
            public TerrainType2 Type2;
            public Unit PlacedUnit;
        }
        public struct Point
        {
            public readonly Terrain _terrain;
            public Terrain Terrain => _terrain;

            public Position Position { get; private set; }

            public TerrainType1 Type1 => Terrain._points[Position.X, Position.Y].Type1;
            public TerrainType2 Type2 => Terrain._points[Position.X, Position.Y].Type2;
            public Unit PlacedUnit => Terrain._points[Position.X, Position.Y].PlacedUnit;

            public Point(Terrain terrain, Position pos)
            {
                if (pos.X < 0 || pos.X >= terrain.Width)
                    throw new ArgumentOutOfRangeException();
                if (pos.Y < 0 || pos.Y >= terrain.Height)
                    throw new ArgumentOutOfRangeException();

                _terrain = terrain;
                Position = pos;
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
                    _points[y, x].Type1 = (TerrainType1)random.Next(4);
                    _points[y, x].Type2 = (TerrainType2)random.Next(3);
                }
            }
        }

        public Point GetPoint(int x, int y)
        {
            return GetPoint(new Position { X = x, Y = y });
        }
        public Point GetPoint(Position pos)
        {
            return new Point(this, pos);
        }
    }
}
