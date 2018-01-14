using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CivModel.TileBuildings;

namespace CivModel
{
    public class TileObjectProduction<T> : Production where T : TileObject
    {
        public TileObjectProduction(Player owner, double totalCost, double capacityPerTurn)
            : base(owner, totalCost, capacityPerTurn)
        {
        }

        public override bool IsPlacable(Terrain.Point point)
        {
            if (point.Unit == null)
                if (point.TileBuilding is CityCenter city)
                    return city.Owner == Owner;
            return false;
        }

        public override void Place(Terrain.Point point)
        {
            if (!IsPlacable(point))
                throw new ArgumentException("Production.Place: point is invalid");

            var unit = (Unit)Activator.CreateInstance(typeof(T), Owner);
            unit.PlacedPoint = point;
        }
    }
}
