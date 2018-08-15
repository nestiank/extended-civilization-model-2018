using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public sealed class Pioneer : Unit
    {
        public Pioneer(Player owner, Terrain.Point point) : base(owner, typeof(Pioneer), point)
        {
        }
    }

    public class PioneerProductionFactory : IActorProductionFactory
    {
        public static PioneerProductionFactory Instance => _instance.Value;
        private static Lazy<PioneerProductionFactory> _instance
            = new Lazy<PioneerProductionFactory>(() => new PioneerProductionFactory());
        private PioneerProductionFactory()
        {
        }

        public Type ResultType => typeof(Pioneer);

        public double TotalLaborCost => 5;
        public double LaborCapacityPerTurn => 2;
        public double TotalGoldCost => 5;
        public double GoldCapacityPerTurn => 2;

        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.Unit == null
                && point.TileBuilding is CityBase
                && point.TileBuilding.Owner == production.Owner;
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new Pioneer(owner, point);
        }
    }
}
