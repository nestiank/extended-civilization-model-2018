using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Zap
{
    public class InfantryDivision : Unit
    {
        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public InfantryDivision(Player owner, Terrain.Point point) : base(owner, typeof(InfantryDivision), point)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
        }
    }

    public class InfantryDivisionProductionFactory : IActorProductionFactory
    {
        private static Lazy<InfantryDivisionProductionFactory> _instance
            = new Lazy<InfantryDivisionProductionFactory>(() => new InfantryDivisionProductionFactory());
        public static InfantryDivisionProductionFactory Instance => _instance.Value;
        private InfantryDivisionProductionFactory()
        {
        }

        public Type ResultType => typeof(InfantryDivision);

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
            return new InfantryDivision(owner, point);
        }
    }
}
