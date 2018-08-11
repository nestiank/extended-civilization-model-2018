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
    public abstract class TileObject : IProductionResult
    {
        /// <summary>
        /// The <see cref="Game"/> object
        /// </summary>
        public Game Game { get; }

        /// <summary>
        /// The value indicating the kind of this object.
        /// </summary>
        public TileTag TileTag { get; }

        /// <summary>
        /// The placed point of this object. <c>null</c> if not placed.
        /// </summary>
        public Terrain.Point? PlacedPoint
        {
            get => _placedPoint;
            set
            {
                if (value != _placedPoint)
                {
                    var oldPoint = _placedPoint;
                    SetPlacedPoint(value);
                    OnChangePlacedPoint(oldPoint);

                    Game.TileObjectEvent.RaiseObservable(obj => obj.TileObjectPlaced(this));
                }
            }
        }
        private Terrain.Point? _placedPoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="TileObject"/> class.
        /// </summary>
        /// <param name="game">The <see cref="CivModel.Game"/> object.</param>
        /// <param name="point">The tile where the object will be.</param>
        /// <param name="tileTag">The <see cref="TileTag"/> of the object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="game"/> is <c>null</c>.</exception>
        public TileObject(Game game, Terrain.Point point, TileTag tileTag)
        {
            Game = game ?? throw new ArgumentNullException(nameof(game));
            TileTag = tileTag;

            SetPlacedPoint(point);
        }

        /// <summary>
        /// Called when production is finished, that is, <see cref="Production.Place(Terrain.Point)" /> is succeeded.
        /// </summary>
        /// <param name="production">The <see cref="Production" /> object that produced this object.</param>
        public virtual void OnAfterProduce(Production production)
        {
            Game.TileObjectEvent.RaiseObservable(obj => obj.TileObjectProduced(this));
            Game.TileObjectEvent.RaiseObservable(obj => obj.TileObjectPlaced(this));
        }

        private void SetPlacedPoint(Terrain.Point? value)
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

        /// <summary>
        /// Called after <see cref="PlacedPoint"/> is changed.
        /// </summary>
        /// <param name="oldPoint">The old value of <see cref="PlacedPoint"/>.</param>
        protected virtual void OnChangePlacedPoint(Terrain.Point? oldPoint)
        {
        }
    }
}
