using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public sealed class Pioneer : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("487BBF97-538A-45CB-A62D-B33E173F8E6F");
        public override Guid Guid => ClassGuid;

        public override double MaxAP => 2;

        public override double GoldLogistics => 10;
        public override double FullLaborLogicstics => 5;

        public Pioneer(Player owner, Terrain.Point point) : base(owner, point)
        {
        }
    }

    public class PioneerProductionFactory : ITileObjectProductionFactory
    {
        public static PioneerProductionFactory Instance => _instance.Value;
        private static Lazy<PioneerProductionFactory> _instance
            = new Lazy<PioneerProductionFactory>(() => new PioneerProductionFactory());
        private PioneerProductionFactory()
        {
        }
        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner, 5, 2, 5, 2);
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
