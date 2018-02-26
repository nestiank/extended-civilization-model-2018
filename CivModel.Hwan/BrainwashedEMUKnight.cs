using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public class BrainwashedEMUKnight : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("6C04C360-C1B9-4633-8269-B0911B1D63DA");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxAP = 2,
            MaxHP = 75,
            AttackPower = 10,
            DefencePower = 7,
            GoldLogistics = 10,
            FullLaborLogistics = 2,
            BattleClassLevel = 1
        };

        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public BrainwashedEMUKnight(Player owner, Terrain.Point point) : base(owner, Constants, point)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
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

        public ActorConstants ActorConstants => BrainwashedEMUKnight.Constants;

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
                && point.TileBuilding is CivModel.Common.CityCenter
                && point.TileBuilding.Owner == production.Owner;
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new BrainwashedEMUKnight(owner, point);
        }
    }
}
