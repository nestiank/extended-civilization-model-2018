using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public class DecentralizedMilitary : Unit
    {
        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public DecentralizedMilitary(Player owner, Terrain.Point point) : base(owner, typeof(DecentralizedMilitary), point)
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
