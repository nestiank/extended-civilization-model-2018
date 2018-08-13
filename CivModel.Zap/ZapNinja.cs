using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Zap
{
    public class ZapNinja : Unit
    {
        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public ZapNinja(Player owner, Terrain.Point point) : base(owner, typeof(ZapNinja), point)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
        }
    }

    public class ZapNinjaProductionFactory : IActorProductionFactory
    {
        private static Lazy<ZapNinjaProductionFactory> _instance
            = new Lazy<ZapNinjaProductionFactory>(() => new ZapNinjaProductionFactory());
        public static ZapNinjaProductionFactory Instance => _instance.Value;
        private ZapNinjaProductionFactory()
        {
        }

        public Type ResultType => typeof(ZapNinja);

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
            return new ZapNinja(owner, point);
        }
    }
}
