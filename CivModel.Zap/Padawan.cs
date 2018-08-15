using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Zap
{
    public class Padawan : Unit
    {
        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public Padawan(Player owner, Terrain.Point point) : base(owner, typeof(Padawan), point)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
        }
    }

    public class PadawanProductionFactory : IActorProductionFactory
    {
        private static Lazy<PadawanProductionFactory> _instance
            = new Lazy<PadawanProductionFactory>(() => new PadawanProductionFactory());
        public static PadawanProductionFactory Instance => _instance.Value;
        private PadawanProductionFactory()
        {
        }

        public Type ResultType => typeof(Padawan);

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
            return new Padawan(owner, point);
        }
    }
}
