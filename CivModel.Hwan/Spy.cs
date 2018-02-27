using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public class Spy : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("B4F8293E-EBF2-47A0-A1BB-DF6A6ECCD766");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxAP = 2,
            MaxHP = 25,
            AttackPower = 5,
            DefencePower = 1,
            GoldLogistics = 10,
            FullLaborForRepair = 2,
            BattleClassLevel = 1
        };

        public Spy(Player owner, Terrain.Point point) : base(owner, Constants, point)
        {

        }

    }

    public class SpyProductionFactory : IActorProductionFactory
    {
        private static Lazy<SpyProductionFactory> _instance
            = new Lazy<SpyProductionFactory>(() => new SpyProductionFactory());
        public static SpyProductionFactory Instance => _instance.Value;
        private SpyProductionFactory()
        {
        }

        public ActorConstants ActorConstants => Spy.Constants;

        public double TotalLaborCost => 32;
        public double LaborCapacityPerTurn => 8;
        public double TotalGoldCost => 60;
        public double GoldCapacityPerTurn => 15;

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
            return new Spy(owner, point);
        }
    }
}
