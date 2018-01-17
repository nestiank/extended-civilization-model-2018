using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class UnicornOrder : Unit
    {
        public override int MaxAP => 4;

        public UnicornOrder(Player owner) : base(owner)
        {
        }
    }

    public class UnicornOrderProductionFactory : ITileObjectProductionFactory
    {
        private static Lazy<UnicornOrderProductionFactory> _instance
            = new Lazy<UnicornOrderProductionFactory>(() => new UnicornOrderProductionFactory());
        public static UnicornOrderProductionFactory Instance => _instance.Value;
        private UnicornOrderProductionFactory()
        {
        }
        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner, 7, 3);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.Unit == null
                && point.TileBuilding is CityCenter
                && point.TileBuilding.Owner == production.Owner;
        }
        public TileObject CreateTileObject(Player owner)
        {
            return new UnicornOrder(owner);
        }
    }
}
