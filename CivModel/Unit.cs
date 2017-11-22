using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    public class Unit
    {
        private readonly Player _owner;
        public Player Owner => _owner;

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
                    p.Terrain.UnplaceUnit(this, p);
                }

                _placedPoint = value;
                if (value.HasValue)
                {
                    value.Value.Terrain.PlaceUnit(this);
                }
            }
        }

        public int MaxAction => 2;
        public int RemainAction { get; private set; }

        public Unit(Player owner)
        {
            _owner = owner;
        }

        public void PreTurn()
        {
            RemainAction = MaxAction;
        }

        public void PostTurn()
        {

        }
    }
}
