using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class ProtoNinja : Unit
    {
        public override int MaxAP => 4;

        public ProtoNinja(Player owner) : base(owner)
        {
        }
    }

    public class ProtoNinjaProductionFactory : ITileObjectProductionFactory
    {
        private static Lazy<ProtoNinjaProductionFactory> _instance
            = new Lazy<ProtoNinjaProductionFactory>(() => new ProtoNinjaProductionFactory());
        public static ProtoNinjaProductionFactory Instance => _instance.Value;
        private ProtoNinjaProductionFactory()
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
            return new ProtoNinja(owner);
        }
    }
}
