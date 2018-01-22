using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class BrainwashedEMUKnight : Unit
    {
        public override int MaxAP => 2;

        public override double MaxHP => 75;

        public override double AttackPower => 10;
        public override double DefencePower => 7;

        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public BrainwashedEMUKnight(Player owner) : base(owner)
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
        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner, 30, 12);
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
