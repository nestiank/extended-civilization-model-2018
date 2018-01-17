using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class AncientSorcerer : Unit
    {
        public override int MaxAP => 4;

        public AncientSorcerer(Player owner) : base(owner)
        {
        }
    }

    public class AncientSorcererProductionFactory : ITileObjectProductionFactory
    {
        private static Lazy<AncientSorcererProductionFactory> _instance
            = new Lazy<AncientSorcererProductionFactory>(() => new AncientSorcererProductionFactory());
        public static AncientSorcererProductionFactory Instance => _instance.Value;
        private AncientSorcererProductionFactory()
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
            return new AncientSorcerer(owner);
        }
    }
}
