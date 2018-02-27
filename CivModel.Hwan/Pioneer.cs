using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public sealed class Pioneer : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("487BBF97-538A-45CB-A62D-B33E173F8E6F");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxAP = 2,
            GoldLogistics = 5,
            LaborLogistics = 0.5
        };

        public Pioneer(Player owner, Terrain.Point point) : base(owner, Constants, point)
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

        public ActorConstants ActorConstants => Pioneer.Constants;

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
