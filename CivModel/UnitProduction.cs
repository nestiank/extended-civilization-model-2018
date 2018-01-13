using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CivModel.TileBuildings;

namespace CivModel
{
    public class UnitProduction : Production
    {
        private readonly Func<Unit> _supplier;

        public UnitProduction(Player owner, double totalCost, double capacityPerTurn, Func<Unit> supplier)
            : base(owner, totalCost, capacityPerTurn)
        {
            _supplier = supplier ?? throw new ArgumentNullException("Production ctor: supplier cannot be null");
        }

        public override bool IsPlacable(Terrain.Point point)
        {
            if (point.Unit != null)
                if (point.TileBuilding is CityCenter city)
                    return city.Owner == Owner;
            return false;
        }

        public override void Place(Terrain.Point point)
        {
            if (!IsPlacable(point))
                throw new ArgumentException("Production.Place: point is invalid");

            var unit = _supplier();
            unit.PlacedPoint = point;
        }
    }
}
