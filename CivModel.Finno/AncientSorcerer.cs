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

        public override int BattleClassLevel => 2;

        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public override IReadOnlyList<IActorAction> SpecialActs => _specialActs;
        private readonly IActorAction[] _specialActs = new IActorAction[1];

        public AncientSorcerer(Player owner, Terrain.Point point) : base(owner, point)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
            _specialActs[0] = new AncientSorcererAction(this);
        }

        private class AncientSorcererAction : IActorAction
        {
            private readonly AncientSorcerer _owner;
            public Actor Owner => _owner;

            public bool IsParametered => true;

            public AncientSorcererAction(AncientSorcerer owner)
            {
                _owner = owner;
            }


            public int GetRequiredAP(Terrain.Point? pt)
            {
                if (pt != null)
                    return -1;
                if (!_owner.PlacedPoint.HasValue)
                    return -1;
                if (pt.Value.Unit.Owner != Owner.Owner)
                    return -1;

                return 1;
            }

            public void Act(Terrain.Point? pt)
            {
                if (pt == null)
                    throw new ArgumentException("pt is invalid");
                if (!_owner.PlacedPoint.HasValue)
                    throw new InvalidOperationException("Actor is not placed yet");
                if (pt.Value.Unit.Owner != Owner.Owner)
                    throw new InvalidOperationException("The Unit is hostile");

                double AmountOfHeal = 0;

                AmountOfHeal = Math.Min(10, Owner.RemainHP - 1);
                pt.Value.Unit.Heal(AmountOfHeal);

                Owner.RemainHP = Owner.RemainHP - AmountOfHeal;

            }
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
            return new TileObjectProduction(this, owner, 50, 15, 20, 4);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.Unit == null
                && point.TileBuilding is CityCenter
                && point.TileBuilding.Owner == production.Owner;
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new AncientSorcerer(owner, point);
        }
    }
}
