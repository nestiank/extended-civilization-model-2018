using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class FakeFortress : TileBuilding
    {
        public FakeFortress(Player owner, Terrain.Point point, Player donator = null)
            : base(owner, typeof(FakeFortress), point, donator)
        {
        }
    }

    public class FakeFortressProductionFactory : ITileBuildingProductionFactory
    {
        public static FakeFortressProductionFactory Instance => _instance.Value;
        private static Lazy<FakeFortressProductionFactory> _instance
            = new Lazy<FakeFortressProductionFactory>(() => new FakeFortressProductionFactory());

        private FakeFortressProductionFactory()
        {
        }

        public Type ResultType => typeof(FakeFortress);

        public Production Create(Player owner)
        {
            return new TileBuildingProduction(this, owner);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.TileBuilding == null && production.Owner.IsAlliedWith(point.TileOwner);
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new FakeFortress(owner, point);
        }
        public TileBuilding CreateDonation(Player owner, Terrain.Point point, Player donator)
        {
            return new FakeFortress(owner, point, donator);
        }
    }

}
