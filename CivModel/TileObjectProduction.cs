using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CivModel.Common;

namespace CivModel
{
    public interface ITileObjectProductionFactory : IProductionFactory
    {
        bool IsPlacable(TileObjectProduction production, Terrain.Point point);
        TileObject CreateTileObject(Player owner);
    }

    public class TileObjectProduction : Production
    {
        private readonly ITileObjectProductionFactory _factory;

        public TileObjectProduction(
            ITileObjectProductionFactory factory, Player owner,
            double totalCost, double capacityPerTurn)
            : base(factory, owner, totalCost, capacityPerTurn)
        {
            _factory = factory;
        }

        public override bool IsPlacable(Terrain.Point point)
        {
            return _factory.IsPlacable(this, point);
        }

        public override void Place(Terrain.Point point)
        {
            if (!IsPlacable(point))
                throw new ArgumentException("Production.Place: point is invalid");

            var obj = _factory.CreateTileObject(Owner);
            obj.PlacedPoint = point;
        }
    }
}
