using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class AncientSorcerer : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("C49FB444-C530-41B8-9D61-A9CF4443A4D6");
        public override Guid Guid => ClassGuid;

        public override int MaxAP => 2;

        public override double MaxHP => 50;

        public override double AttackPower => 15;
        public override double DefencePower => 5;

        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public AncientSorcerer(Player owner) : base(owner)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
        }
    }

    public class AncientSorcererProductionFactory : ITileObjectProductionFactory
    {
        private static Lazy<AncientSorcererProductionFactory> _instance
            = new Lazy<AncientSorcererProductionFactory>(() => new AncientSorcererProductionFactory());
        public static AncientSorcererProductionFactory Instance => _instance.Value;
        private AncientSorcererProductionFactory()
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
        public TileObject CreateTileObject(Player owner)
        {
            return new AncientSorcerer(owner);
        }
    }
}
