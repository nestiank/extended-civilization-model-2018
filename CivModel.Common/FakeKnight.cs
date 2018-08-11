using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public sealed class FakeKnight : Unit
    {
        public override IActorAction HoldingAttackAct => _holdingAttackAct;
        private readonly IActorAction _holdingAttackAct;

        public override IActorAction MovingAttackAct => _movingAttackAct;
        private readonly IActorAction _movingAttackAct;

        public override IReadOnlyList<IActorAction> SpecialActs => _specialActs;
        private readonly IActorAction[] _specialActs = new IActorAction[1];

        public FakeKnight(Player owner, Terrain.Point point)
            : base(owner, typeof(FakeKnight), point)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
            _specialActs[0] = new MindControlSkill(this);
        }

        private class MindControlSkill : IActorAction
        {
            public Actor Owner => _owner;
            private readonly FakeKnight _owner;

            public bool IsParametered => true;

            public MindControlSkill(FakeKnight owner)
            {
                _owner = owner;
            }

            public ActionPoint GetRequiredAP(Terrain.Point origin, Terrain.Point? target)
            {
                if (CheckError(origin, target) != null)
                    return double.NaN;

                return 1;
            }

            public void Act(Terrain.Point? pt)
            {
                if (!_owner.PlacedPoint.HasValue)
                    throw new InvalidOperationException("Actor is not placed yet");
                if (CheckError(_owner.PlacedPoint.Value, pt) is Exception e)
                    throw e;

                new ControlHijackEffect(pt.Value.Unit, Owner.Owner).EffectOn();
                Owner.ConsumeAP(GetRequiredAP(_owner.PlacedPoint.Value, pt));
            }

            private Exception CheckError(Terrain.Point origin, Terrain.Point? target)
            {
                if (target == null)
                    return new ArgumentNullException(nameof(target));

                if (target.Value.Unit is Unit unit && unit.Owner != Owner.Owner)
                {
                    if (target.Value.TileBuilding != null)
                        return new InvalidOperationException("the ownership of unit on TileBuilding cannot be changed");

                    return null;
                }
                else
                {
                    return new ArgumentException("there is no target of skill");
                }
            }
        }
    }

    public class FakeKnightProductionFactory : IActorProductionFactory
    {
        private static Lazy<FakeKnightProductionFactory> _instance
            = new Lazy<FakeKnightProductionFactory>(() => new FakeKnightProductionFactory());
        public static FakeKnightProductionFactory Instance => _instance.Value;
        private FakeKnightProductionFactory()
        {
        }

        public Type ResultType => typeof(FakeKnight);

        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.Unit == null
                && point.TileBuilding is CityBase
                && point.TileBuilding.Owner == production.Owner;
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new FakeKnight(owner, point);
        }
    }
}
