using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class GenghisKhan : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("DCB724DA-430E-4B13-8703-9517F173FE4B");
        public override Guid Guid => ClassGuid;

        public override int MaxAP => 2;

        public override double MaxHP => 50;

        public override double AttackPower => 35;
        public override double DefencePower => 10;

        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public GenghisKhan(Player owner, Terrain.Point point) : base(owner, point)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
        }
    }

    public class GenghisKhanProductionFactory : ITileObjectProductionFactory
    {
        private static Lazy<GenghisKhanProductionFactory> _instance
            = new Lazy<GenghisKhanProductionFactory>(() => new GenghisKhanProductionFactory());
        public static GenghisKhanProductionFactory Instance => _instance.Value;
        private GenghisKhanProductionFactory()
        {
        }
        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner, 100, 20, 50, 10);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.Unit == null
                && point.TileBuilding is CityCenter
                && point.TileBuilding.Owner == production.Owner;
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new GenghisKhan(owner, point);
        }
    }
}
