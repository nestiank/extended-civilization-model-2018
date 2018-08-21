using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public interface ISpyRelatedQuest
    {
        void OnSpyAction(Spy spy);
    }

    public class Spy : Unit
    {
        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public override IReadOnlyList<IActorAction> SpecialActs => _specialActs;
        private readonly IActorAction[] _specialActs = new IActorAction[1];

        public Spy(Player owner, Terrain.Point point) : base(owner, typeof(Spy), point)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
            _specialActs[0] = new SpyAction(this);

            this.IsCloacking = true;
        }

        private class SpyAction : IActorAction
        {
            private readonly Spy _owner;
            public Actor Owner => _owner;

            public bool IsParametered => false;

            public SpyAction(Spy owner)
            {
                _owner = owner;
            }

            public ActionPoint GetRequiredAP(Terrain.Point origin, Terrain.Point? target)
            {
                if (CheckError(origin, target) != null)
                    return double.NaN;

                return 1;
            }

            private Exception CheckError(Terrain.Point origin, Terrain.Point? target)
            {
                if (target != null)
                    return new ArgumentException("target is invalid");
                if (_owner.Owner.IsAlliedWithOrNull(origin.TileOwner))
                    return new InvalidOperationException("Actor is not placed in Hostile");

                return null;
            }

            public void Act(Terrain.Point? target)
            {
                if (!_owner.PlacedPoint.HasValue)
                    throw new InvalidOperationException("Actor is not placed yet");
                var origin = _owner.PlacedPoint.Value;

                if (CheckError(origin, target) is Exception e)
                    throw e;

                ActionPoint Ap = GetRequiredAP(origin, target);
                if (!Owner.CanConsumeAP(Ap))
                    throw new InvalidOperationException("Not enough Ap");

                foreach (var quest in _owner.Owner.Quests)
                {
                    if (quest is ISpyRelatedQuest q)
                        q.OnSpyAction(_owner);
                }

                Owner.ConsumeAP(Ap);
                _owner.Destroy();
            }
        }
    }

    public class SpyProductionFactory : IActorProductionFactory
    {
        private static Lazy<SpyProductionFactory> _instance
            = new Lazy<SpyProductionFactory>(() => new SpyProductionFactory());
        public static SpyProductionFactory Instance => _instance.Value;
        private SpyProductionFactory()
        {
        }

        public Type ResultType => typeof(Spy);

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
            return new Spy(owner, point);
        }
    }
}
