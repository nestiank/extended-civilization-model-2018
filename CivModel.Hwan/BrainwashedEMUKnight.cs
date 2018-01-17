using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class BrainwashedEMUKnight : Unit
    {
        public override int MaxAP => 4;

        public BrainwashedEMUKnight(Player owner) : base(owner)
        {
        }
    }

    public class BrainwashedEMUKnightProductionFactory : ITileObjectProductionFactory
    {
        private static Lazy<BrainwashedEMUKnightProductionFactory> _instance
            = new Lazy<BrainwashedEMUKnightProductionFactory>(() => new BrainwashedEMUKnightProductionFactory());
        public static BrainwashedEMUKnightProductionFactory Instance => _instance.Value;
        private BrainwashedEMUKnightProductionFactory()
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
            return new BrainwashedEMUKnight(owner);
        }
    }
}
