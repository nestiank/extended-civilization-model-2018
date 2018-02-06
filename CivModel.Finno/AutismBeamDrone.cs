using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class AutismBeamDrone : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("B1637348-A97F-4D7F-B160-B82E4695F2C3");
        public override Guid Guid => ClassGuid;

        public override int MaxAP => 2;

        public override double MaxHP => 35;

        public override double AttackPower => 20;
        public override double DefencePower => 5;

        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public AutismBeamDrone(Player owner, Terrain.Point point) : base(owner, point)
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
            return new TileObjectProduction(this, owner, 75, 20, 50, 10);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.Unit == null
                && point.TileBuilding is CityCenter
                && point.TileBuilding.Owner == production.Owner;
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new AutismBeamDrone(owner, point);
        }
    }
}
