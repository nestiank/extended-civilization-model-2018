using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace CivModel
{
    public enum TerrainType
    {
        Plain, Ocean, Mount, Forest, Swamp, Tundra, Ice, Hill
    }

    public partial class Terrain
    {
        private struct Point_t
        {
            public TerrainType Type;
            public TileObject[] PlacedObjects;
        }

        private Point_t[,] _points;

        public readonly int _width, _height;
        public int Width => _width;
        public int Height => _height;

        public Terrain(int width, int height)
        {
            _width = width;
            _height = height;
            _points = new Point_t[_height, _width];

            for (int y = 0; y < _height; ++y)
            {
                for (int x = 0; x < _width; ++x)
                {
                    _points[y, x].Type = TerrainType.Plain;
                }
            }
        }

        public Terrain()
        {
            using (var file = File.OpenText("map.txt"))
            {
                int[] wh = file.ReadLine().Split(' ').Select(str => Convert.ToInt32(str)).ToArray();
                _width = wh[0];
                _height = wh[1];
                _points = new Point_t[_height, _width];

                for (int y = 0; y < _height; ++y)
                {
                    string line = file.ReadLine();
                    for (int x = 0; x < _width; ++x)
                    {
                        if (x >= line.Length)
                            throw new InvalidDataException();

                        int len = Enum.GetNames(typeof(TileTag)).Length;
                        _points[y, x].PlacedObjects = new TileObject[len];

                        int idx = "POMFSTIH".IndexOf(line[x]);
                        if (idx == -1)
                            throw new InvalidDataException();

                        _points[y, x].Type = (TerrainType)idx;
                    }
                }
            }
        }

        public void Save()
        {
            using (var file = File.CreateText("map.txt"))
            {
                file.WriteLine(Width + " " + Height);
                for (int y = 0; y < _height; ++y)
                {
                    for (int x = 0; x < _width; ++x)
                    {
                        file.Write("POMFSTIH"[(int)_points[y, x].Type]);
                    }
                    file.WriteLine();
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

        /// <summary>
        /// this function is used by the setter of <see cref="TileObject.PlacedPoint"/>.
        /// In general case you should use <see cref="TileObject.PlacedPoint"/> instead.
        /// </summary>
        internal void PlaceObject(TileObject obj)
        {
            var p = obj.PlacedPoint
                ?? throw new ArgumentException("obj.PlacedPoint is null");

            TileObject prev = _points[p.Position.X, p.Position.Y].PlacedObjects[(int)obj.TileTag];
            if (prev != null)
                throw new InvalidOperationException("PlaceObject() is called while another object exists");

            _points[p.Position.X, p.Position.Y].PlacedObjects[(int)obj.TileTag] = obj;
        }

        /// <summary>
        /// this function is used by the setter of <see cref="TileObject.PlacedPoint"/>.
        /// In general case you should use <see cref="TileObject.PlacedPoint"/> instead.
        /// </summary>
        internal void UnplaceObject(TileObject obj, Point p)
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
