using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public sealed class AncientFinnoGermaniumMine : TileBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("3F74C92A-A927-489E-AD81-D18D14BDF65B");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxHP = 30,
            GoldLogistics = 20,
            FullLaborLogistics = 10
        };

        public AncientFinnoGermaniumMine(Player owner, Terrain.Point point) : base(owner, Constants, point) { }

        public override void PostTurn()
        {
            this.RemainHP = Math.Min(30, (this.RemainHP + 5));
        }
    }

    public class AncientFinnoGermaniumMineProductionFactory : ITileObjectProductionFactory
    {
        public static AncientFinnoGermaniumMineProductionFactory Instance => _instance.Value;
        private static Lazy<AncientFinnoGermaniumMineProductionFactory> _instance
            = new Lazy<AncientFinnoGermaniumMineProductionFactory>(() => new AncientFinnoGermaniumMineProductionFactory());
        private AncientFinnoGermaniumMineProductionFactory()
        {
        }

        public ActorConstants ActorConstants => AncientFinnoGermaniumMine.Constants;

        public double TotalLaborCost => 10;
        public double LaborCapacityPerTurn => 10;
        public double TotalGoldCost => 10;
        public double GoldCapacityPerTurn => 10;

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
            return new AncientFinnoGermaniumMine(owner, point);
        }
    }
}
