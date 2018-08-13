using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public class BrainwashedEMUKnight : Unit
    {
        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public BrainwashedEMUKnight(Player owner, Terrain.Point point) : base(owner, typeof(BrainwashedEMUKnight), point)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
        }
    }

    public class BrainwashedEMUKnightProductionFactory : IActorProductionFactory
    {
        private static Lazy<BrainwashedEMUKnightProductionFactory> _instance
            = new Lazy<BrainwashedEMUKnightProductionFactory>(() => new BrainwashedEMUKnightProductionFactory());
        public static BrainwashedEMUKnightProductionFactory Instance => _instance.Value;
        private BrainwashedEMUKnightProductionFactory()
        {
        }

        public Type ResultType => typeof(BrainwashedEMUKnight);

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
            return new BrainwashedEMUKnight(owner, point);
        }
    }
}
