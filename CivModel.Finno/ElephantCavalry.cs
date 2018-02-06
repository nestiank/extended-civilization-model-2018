using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class ElephantCavalry : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("8AC71759-3B58-4637-9F09-4F483EB0F4B8");
        public override Guid Guid => ClassGuid;

        public override int MaxAP => 2;

        public override double MaxHP => 50;

        public override double AttackPower => 17;
        public override double DefencePower => 5;

        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public ElephantCavalry(Player owner, Terrain.Point point) : base(owner, point)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
        }
    }

    public class ElephantCavalryProductionFactory : ITileObjectProductionFactory
    {
        private static Lazy<ElephantCavalryProductionFactory> _instance
            = new Lazy<ElephantCavalryProductionFactory>(() => new ElephantCavalryProductionFactory());
        public static ElephantCavalryProductionFactory Instance => _instance.Value;
        private ElephantCavalryProductionFactory()
        {
        }
        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner, 50, 15);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.Unit == null
                && point.TileBuilding is CityCenter
                && point.TileBuilding.Owner == production.Owner;
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new ElephantCavalry(owner, point);
        }
    }
}
