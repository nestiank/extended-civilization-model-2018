using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class ProtoNinja : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("87710C32-94A3-4A9D-92ED-8BB29EE2B475");
        public override Guid Guid => ClassGuid;

        public override int MaxAP => 2;

        public override double MaxHP => 35;

        public override double AttackPower => 20;
        public override double DefencePower => 5;

        public override int BattleClassLevel => 3;

        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public ProtoNinja(Player owner, Terrain.Point point) : base(owner, point)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
        }
    }

    public class ProtoNinjaProductionFactory : ITileObjectProductionFactory
    {
        private static Lazy<ProtoNinjaProductionFactory> _instance
            = new Lazy<ProtoNinjaProductionFactory>(() => new ProtoNinjaProductionFactory());
        public static ProtoNinjaProductionFactory Instance => _instance.Value;
        private ProtoNinjaProductionFactory()
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
            return new ProtoNinja(owner, point);
        }
    }
}
