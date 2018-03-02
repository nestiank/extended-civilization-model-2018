using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Zap
{
    public class DecentralizedMilitary : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("FCE532E4-E5B2-4569-B91E-A998EBC72689");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxAP = 2,
            MaxHP = 25,
            AttackPower = 3,
            DefencePower = 2,
            GoldLogistics = 10,
            FullLaborForRepair = 2,
            BattleClassLevel = 1
        };

        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public DecentralizedMilitary(Player owner, Terrain.Point point) : base(owner, Constants, point)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
        }
    }

    public class DecentralizedMilitaryProductionFactory : ITileObjectProductionFactory
    {
        private static Lazy<DecentralizedMilitaryProductionFactory> _instance
            = new Lazy<DecentralizedMilitaryProductionFactory>(() => new DecentralizedMilitaryProductionFactory());
        public static DecentralizedMilitaryProductionFactory Instance => _instance.Value;
        private DecentralizedMilitaryProductionFactory()
        {
        }

        public Type ResultType => typeof(DecentralizedMilitary);
        public ActorConstants ActorConstants => DecentralizedMilitary.Constants;

        public double TotalLaborCost => 15;
        public double LaborCapacityPerTurn => 8;
        public double TotalGoldCost => 30;
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
            return new DecentralizedMilitary(owner, point);
        }
    }
}
