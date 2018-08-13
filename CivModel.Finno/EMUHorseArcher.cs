using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public class EMUHorseArcher : Unit
    {
        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public EMUHorseArcher(Player owner, Terrain.Point point) : base(owner, typeof(EMUHorseArcher), point)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
        }
    }

    public class EMUHorseArcherProductionFactory : IActorProductionFactory
    {
        private static Lazy<EMUHorseArcherProductionFactory> _instance
            = new Lazy<EMUHorseArcherProductionFactory>(() => new EMUHorseArcherProductionFactory());
        public static EMUHorseArcherProductionFactory Instance => _instance.Value;
        private EMUHorseArcherProductionFactory()
        {
        }

        public Type ResultType => typeof(EMUHorseArcher);

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
            return new EMUHorseArcher(owner, point);
        }
    }
}
