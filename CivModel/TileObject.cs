using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// The value indicating the kind of <see cref="TileObject"/>.
    /// </summary>
    public enum TileTag
    {
        /// <summary>
        /// Tag for <see cref="Unit"/> object
        /// </summary>
        Unit,
        /// <summary>
        /// Tag for <see cref="TileBuilding"/> object
        /// </summary>
        TileBuilding
    }

    /// <summary>
    /// Represents an object which can be placed on <see cref="Terrain.Point"/>.
    /// </summary>
    public abstract class TileObject : IGuidTaggedObject
    {
        /// <summary>
        /// The unique identifier of this class.
        /// </summary>
        public abstract Guid Guid { get; }

        /// <summary>
        /// The value indicating the kind of this object.
        /// </summary>
        public TileTag TileTag => _tileTag;
        private readonly TileTag _tileTag;

        /// <summary>
        /// The placed point of this object. <c>null</c> if not placed.
        /// </summary>
        public Terrain.Point? PlacedPoint
        {
            get => _placedPoint;
            set
            {
                if (_placedPoint.HasValue)
                {
                    var p = _placedPoint.Value;
                    _placedPoint = null;
                    p.Terrain.UnplaceObject(this, p);
                }

                _placedPoint = value;
                if (value.HasValue)
                {
                    value.Value.Terrain.PlaceObject(this);
                }
            }
        }

        private Terrain.Point? _placedPoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="TileObject"/> class.
        /// </summary>
        /// <param name="tileTag">The <see cref="TileTag"/> of a object.</param>
        public TileObject(TileTag tileTag)
        {
            _tileTag = tileTag;
        }
    }
}
