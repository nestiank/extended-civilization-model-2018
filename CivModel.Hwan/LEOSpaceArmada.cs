using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class LEOSpaceArmada : Unit
    {
        public override int MaxAP => 4;

        public override double MaxHP => 50;

        public override double AttackPower => 15;
        public override double DefencePower => 5;

        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public LEOSpaceArmada(Player owner) : base(owner)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
        }
    }

    public class LEOSpaceArmadaProductionFactory : ITileObjectProductionFactory
    {
        private static Lazy<LEOSpaceArmadaProductionFactory> _instance
            = new Lazy<LEOSpaceArmadaProductionFactory>(() => new LEOSpaceArmadaProductionFactory());
        public static LEOSpaceArmadaProductionFactory Instance => _instance.Value;
        private LEOSpaceArmadaProductionFactory()
        {
        }
        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner, 50, 20);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.Unit == null
                && point.TileBuilding is CityCenter
                && point.TileBuilding.Owner == production.Owner;
        }
        public TileObject CreateTileObject(Player owner)
        {
            return new LEOSpaceArmada(owner);
        }
    }
}
