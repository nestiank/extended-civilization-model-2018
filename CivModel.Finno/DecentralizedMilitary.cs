using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public class DecentralizedMilitary : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("E1B2CAD9-56D5-427B-AEB7-6B291AD4F3D1");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxAP = 2,
            MaxHP = 50,
            AttackPower = 13,
            DefencePower = 5,
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

    public class DecentralizedMilitaryProductionFactory : IActorProductionFactory
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
