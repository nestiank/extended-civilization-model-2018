using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class ElephantCavalry : Unit
    {
        public override int MaxAP => 4;

        public ElephantCavalry(Player owner) : base(owner)
        {
        }
    }

    public class ElephantCavalryProductionFactory : ITileObjectProductionFactory
    {
        private static Lazy<ElephantCavalryProductionFactory> _instance
            = new Lazy<ElephantCavalryProductionFactory>(() => new ElephantCavalryProductionFactory());
        public static ElephantCavalryProductionFactory Instance => _instance.Value;
        private ElephantCavalryProductionFactory()
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
            return new ElephantCavalry(owner);
        }
    }
}
