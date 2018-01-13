using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    public enum TileTag
    {
        Unit,
        TileBuilding
    }

    public abstract class TileObject
    {
        public readonly TileTag _tileTag;
        public TileTag TileTag => _tileTag;

        private Terrain.Point? _placedPoint;
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

        public TileObject(TileTag tileTag)
        {
            _tileTag = tileTag;
        }
    }
}
