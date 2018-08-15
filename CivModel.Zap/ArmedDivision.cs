using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Zap
{
    public class ArmedDivision : Unit
    {
        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public ArmedDivision(Player owner, Terrain.Point point) : base(owner, typeof(ArmedDivision), point)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
        }
    }

    public class ArmedDivisionProductionFactory : IActorProductionFactory
    {
        private static Lazy<ArmedDivisionProductionFactory> _instance
            = new Lazy<ArmedDivisionProductionFactory>(() => new ArmedDivisionProductionFactory());
        public static ArmedDivisionProductionFactory Instance => _instance.Value;
        private ArmedDivisionProductionFactory()
        {
        }

        public Type ResultType => typeof(ArmedDivision);

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
