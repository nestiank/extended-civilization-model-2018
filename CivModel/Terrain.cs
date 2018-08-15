using System;
using System.Collections.Generic;
using System.Linq;

namespace CivModel
{
    /// <summary>
    /// Represents a terrain of a game.
    /// </summary>
    public sealed partial class Terrain
    {
        private struct Point_t
        {
            public TerrainType Type;
            public Player TileOwner;
            public TileObject[] PlacedObjects;
        }

        private Point_t[] _points;

        /// <summary>
        /// Gets all tiles of this terrain.
        /// </summary>
        public IEnumerable<Point> AllTiles =>
            Enumerable.Range(0, Width * Height).Select(index => GetPoint(index));

        /// <summary>
        /// The width of this terrain.
        /// </summary>
        public int Width => _width;
        private readonly int _width;

        /// <summary>
        /// The height of this terrain.
        /// </summary>
        public int Height => _height;
        private readonly int _height;

        /// <summary>
        /// Initializes a new instance of the <see cref="Terrain"/> class.
        /// </summary>
        /// <param name="width">The width of a terrain.</param>
        /// <param name="height">The height of a terrain.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="width"/> is not positive
        /// or
        /// <paramref name="height"/> is not positive
        /// </exception>
        public Terrain(int width, int height)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width", width, "width is not positive");
            if (height <= 0)
                throw new ArgumentOutOfRangeException("height", height, "height is not positive");

            _width = width;
            _height = height;

            _points = new Point_t[width * height];

            var random = new Random();
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int len = Enum.GetNames(typeof(TileTag)).Length;
                    _points[y * width + x].PlacedObjects = new TileObject[len];

                    len = Enum.GetNames(typeof(TerrainType)).Length;
                    _points[y * width + x].Type = (TerrainType)random.Next(len);
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="Point"/> from point index.
        /// Point index is equal to <c>y * <see cref="Width"/> + x</c>.
        /// </summary>
        /// <param name="index">The index of point.</param>
        /// <returns>The <see cref="Point"/> object</returns>
        /// <exception cref="ArgumentOutOfRangeException">index is invalid</exception>
        /// <seealso cref="Point.Index"/>
        public Point GetPoint(int index)
        {
            int y = index / Width;
            if (y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException(nameof(index), "index is invalid");
            int x = index % Width;
            return GetPoint(x, y);
        }

        /// <summary>
        /// Gets the <see cref="Point"/> from physical coordinates.
        /// </summary>
        /// <param name="x">X in physical coordinate.</param>
        /// <param name="y">Y in physical coordinate.</param>
        /// <returns>the <see cref="Point"/> object</returns>
        /// <exception cref="ArgumentException">coordinate is invalid.</exception>
        public Point GetPoint(int x, int y)
        {
            return GetPoint(Position.FromPhysical(x, y));
        }

        /// <summary>
        /// Gets the <see cref="Point"/> from logical coordinates.
        /// </summary>
        /// <param name="a">A in logical coordinate.</param>
        /// <param name="b">B in logical coordinate.</param>
        /// <param name="c">C in logical coordinate.</param>
        /// <returns>the <see cref="Point"/> object</returns>
        /// <exception cref="ArgumentException">coordinate is invalid.</exception>
        public Point GetPoint(int a, int b, int c)
        {
            return GetPoint(Position.FromLogical(a, b, c));
        }

        /// <summary>
        /// Gets the <see cref="Point"/> from <see cref="Position"/>
        /// </summary>
        /// <param name="pos">The <see cref="Position"/> object.</param>
        /// <returns>the <see cref="Point"/> object</returns>
        public Point GetPoint(Position pos)
        {
            return new Point(this, pos);
        }

        /// <summary>
        /// Tries to get the <see cref="Point"/> from <see cref="Position"/>
        /// </summary>
        /// <param name="pos">The <see cref="Position"/> object.</param>
        /// <returns>The <see cref="Point"/> object. If <paramref name="pos"/> is invalid, <c>null</c>.</returns>
        public Point? TryGetPoint(Position pos)
        {
            if (IsValidPosition(pos))
                return new Point(this, pos);
            else
                return null;
        }

        /// <summary>
        /// this function is used by the setter of <see cref="TileObject.PlacedPoint"/>.
        /// In general case you should use <see cref="TileObject.PlacedPoint"/> instead.
        /// </summary>
        internal void PlaceObject(TileObject obj)
        {
            var p = obj.PlacedPoint
                ?? throw new ArgumentException("obj.PlacedPoint is null");

            TileObject prev = p.GetTileObject(obj.TileTag);
            if (prev != null)
                throw new InvalidOperationException("PlaceObject() is called while another object exists");

            p.SetTileObject(obj);
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

            if (p.GetTileObject(obj.TileTag) != obj)
                throw new ArgumentException("obj is not on the specified point");

            p.UnsetTileObject(obj.TileTag);
        }

        /// <summary>
        /// Determines whether the specified position is vaild.
        /// </summary>
        /// <param name="pos">The <see cref="Position"/> object.</param>
        /// <returns>
        ///   <c>true</c> if the specified position is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidPosition(Position pos)
        {
            if (pos.Y < 0 || pos.Y >= Height)
                return false;
            return true;
        }
    }
}
