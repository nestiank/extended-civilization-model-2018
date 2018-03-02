using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Zap
{
    public class ArmedDivision : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("68AE9D0F-3492-427A-B4F4-8EA84C14DA4A");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxAP = 2,
            MaxHP = 25,
            AttackPower = 9,
            DefencePower = 3,
            GoldLogistics = 20,
            FullLaborForRepair = 2,
            BattleClassLevel = 2
        };

        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public ArmedDivision(Player owner, Terrain.Point point) : base(owner, Constants, point)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
        }
    }

    public class ArmedDivisionProductionFactory : ITileObjectProductionFactory
    {
        private static Lazy<ArmedDivisionProductionFactory> _instance
            = new Lazy<ArmedDivisionProductionFactory>(() => new ArmedDivisionProductionFactory());
        public static ArmedDivisionProductionFactory Instance => _instance.Value;
        private ArmedDivisionProductionFactory()
        {
        }

        public Type ResultType => typeof(ArmedDivision);
        public ActorConstants ActorConstants => ArmedDivision.Constants;

        public double TotalLaborCost => 30;
        public double LaborCapacityPerTurn => 15;
        public double TotalGoldCost => 50;
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
            return new ArmedDivision(owner, point);
        }
    }
}
