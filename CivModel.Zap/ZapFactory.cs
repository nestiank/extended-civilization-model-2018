using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Zap
{
    public sealed class ZapFactory : TileBuilding
    {
        public ZapFactory(Player owner, Terrain.Point point, Player donator = null)
            : base(owner, typeof(ZapFactory), point, donator)
        {
        }
    }

    public class ZapFactoryProductionFactory : ITileBuildingProductionFactory
    {
        public static ZapFactoryProductionFactory Instance => _instance.Value;
        private static Lazy<ZapFactoryProductionFactory> _instance
            = new Lazy<ZapFactoryProductionFactory>(() => new ZapFactoryProductionFactory());
        private ZapFactoryProductionFactory()
        {
        }

        public Type ResultType => typeof(ZapFactory);

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
            return new ZapFactory(owner, point);
        }
        public TileBuilding CreateDonation(Player owner, Terrain.Point point, Player donator)
        {
            return new ZapFactory(owner, point, donator);
        }
    }
}
