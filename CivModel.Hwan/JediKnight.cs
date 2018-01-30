using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class JediKnight : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("4DCCABFE-74C5-44C9-AB5E-6340F6DF75C5");
        public override Guid Guid => ClassGuid;

        public override int MaxAP => 2;

        public override double MaxHP => 30;

        public override double AttackPower => 25;
        public override double DefencePower => 5;

        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public JediKnight(Player owner) : base(owner)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
        }
    }

    public class JediKnightProductionFactory : ITileObjectProductionFactory
    {
        private static Lazy<JediKnightProductionFactory> _instance
            = new Lazy<JediKnightProductionFactory>(() => new JediKnightProductionFactory());
        public static JediKnightProductionFactory Instance => _instance.Value;
        private JediKnightProductionFactory()
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
            return new JediKnight(owner);
        }
    }
}
