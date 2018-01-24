using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class AutismBeamDrone : Unit
    {
        public override int MaxAP => 2;

        public override double MaxHP => 35;

        public override double AttackPower => 20;
        public override double DefencePower => 5;

        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public AutismBeamDrone(Player owner) : base(owner)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
        }
    }

    public class AutismBeamDroneFactory : ITileObjectProductionFactory
    {
        private static Lazy<AutismBeamDroneFactory> _instance
            = new Lazy<AutismBeamDroneFactory>(() => new AutismBeamDroneFactory());
        public static AutismBeamDroneFactory Instance => _instance.Value;
        private AutismBeamDroneFactory()
        {
        }
        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner, 75, 20);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.Unit == null
                && point.TileBuilding is CityCenter
                && point.TileBuilding.Owner == production.Owner;
        }
        public TileObject CreateTileObject(Player owner)
        {
            return new AutismBeamDrone(owner);
        }
    }
}
