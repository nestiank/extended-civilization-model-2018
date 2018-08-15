using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public class ProtoNinja : Unit
    {
        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public override IReadOnlyList<IActorAction> SpecialActs => _specialActs;
        private readonly IActorAction[] _specialActs = new IActorAction[1];

        public ProtoNinja(Player owner, Terrain.Point point) : base(owner, typeof(ProtoNinja), point)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
            _specialActs[0] = new ProtoNinjaAction(this);
        }

        private class ProtoNinjaAction : IActorAction
        {
            private readonly ProtoNinja _owner;
            public Actor Owner => _owner;

            public bool IsParametered => true;

            public ProtoNinjaAction(ProtoNinja owner)
            {
                _owner = owner;
            }

            public int LastSkillCalled = -3;

            public ActionPoint GetRequiredAP(Terrain.Point origin, Terrain.Point? target)
            {
                if (CheckError(origin, target) != null)
                    return double.NaN;

                return 1;
            }

            private Exception CheckError(Terrain.Point origin, Terrain.Point? target)
            {
                if (target == null)
                    return new ArgumentNullException(nameof(target));
                if (Owner.Owner.Game.TurnNumber <= LastSkillCalled + 2)
                    return new InvalidOperationException("Skill is not turned on");
                if (target.Value.Unit == null)
                    return new InvalidOperationException("There is no target");
                if (Math.Max(Math.Max(Math.Abs(target.Value.Position.A - Owner.PlacedPoint.Value.Position.A), Math.Abs(target.Value.Position.B - Owner.PlacedPoint.Value.Position.B)), Math.Abs(target.Value.Position.C - Owner.PlacedPoint.Value.Position.C)) > 2)
                    return new InvalidOperationException("Too far to attack");
                if (target.Value.Unit.Owner == Owner.Owner)
                    return new InvalidOperationException("The Unit is friendly");
                if (target.Value.TileBuilding != null)
                    return new InvalidOperationException("The Unit is in Building");
                if (target.Value.Unit.BattleClassLevel > 3)
                    return new InvalidOperationException("The Unit's ClassLevel is more then limit");

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

                Owner.AttackTo(target.Value.Unit.MaxHP, target.Value.Unit, 0, true, true);

                if (target.Value.Unit == null && Owner != null)
                {
                    Owner.PlacedPoint = target;
                }
                LastSkillCalled = Owner.Owner.Game.TurnNumber;
                Owner.ConsumeAP(Ap);
            }
        }        
    }

    public class ProtoNinjaProductionFactory : IActorProductionFactory
    {
        private static Lazy<ProtoNinjaProductionFactory> _instance
            = new Lazy<ProtoNinjaProductionFactory>(() => new ProtoNinjaProductionFactory());
        public static ProtoNinjaProductionFactory Instance => _instance.Value;
        private ProtoNinjaProductionFactory()
        {
        }

        public Type ResultType => typeof(ProtoNinja);

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
            return new ProtoNinja(owner, point);
        }
    }
}

