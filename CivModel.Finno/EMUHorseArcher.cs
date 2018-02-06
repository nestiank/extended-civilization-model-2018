using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class EMUHorseArcher : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("2BCC409A-567A-4198-83BB-BD85E6B74A68");
        public override Guid Guid => ClassGuid;

        public override int MaxAP => 2;

        public override double MaxHP => 75;

        public override double AttackPower => 10;
        public override double DefencePower => 7;

        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public EMUHorseArcher(Player owner, Terrain.Point point) : base(owner, point)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
        }
    }

    public class EMUHorseArcherProductionFactory : ITileObjectProductionFactory
    {
        private static Lazy<EMUHorseArcherProductionFactory> _instance
            = new Lazy<EMUHorseArcherProductionFactory>(() => new EMUHorseArcherProductionFactory());
        public static EMUHorseArcherProductionFactory Instance => _instance.Value;
        private EMUHorseArcherProductionFactory()
        {
        }
        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner, 30, 10, 10, 2);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.Unit == null
                && point.TileBuilding is CityCenter
                && point.TileBuilding.Owner == production.Owner;
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new EMUHorseArcher(owner, point);
        }
    }
}
