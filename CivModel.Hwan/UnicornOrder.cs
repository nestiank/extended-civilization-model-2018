using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public class UnicornOrder : Unit
    {
        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public override IReadOnlyList<IActorAction> SpecialActs => _specialActs;
        private readonly IActorAction[] _specialActs = new IActorAction[1];

        public UnicornOrder(Player owner, Terrain.Point point) : base(owner, typeof(UnicornOrder), point)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
            _specialActs[0] = new UnicornOrderAction(this);
        }

        private class UnicornOrderAction : IActorAction
        {
            private readonly UnicornOrder _owner;
            public Actor Owner => _owner;

            public bool IsParametered => true;

            public UnicornOrderAction(UnicornOrder owner)
            {
                _owner = owner;
            }

            public int LastSkillCalled = -2;

            public ActionPoint GetRequiredAP(Terrain.Point origin, Terrain.Point? target)
            {
                if (CheckError(origin, target) != null)
                    return double.NaN;

                return 1;
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


                Owner.PlacedPoint = target;

                LastSkillCalled = Owner.Owner.Game.TurnNumber;
                Owner.ConsumeAP(Ap);
            }

            private Exception CheckError(Terrain.Point origin, Terrain.Point? target)
            {
                if (target == null)
                    return new ArgumentNullException(nameof(target));
                if (Owner.Owner.Game.TurnNumber <= LastSkillCalled + 1)
                    return new InvalidOperationException("Skill is not turned on");
                if (target.Value.Unit != null)
                    return new InvalidOperationException("Can't go that way");
                if (!this.DirectionCheck(origin, target.Value))
                    return new InvalidOperationException("Can't go that way");

                return null;
            }

            private bool DirectionCheck(Terrain.Point origin, Terrain.Point target)
            {
                if (target.Position.A != origin.Position.A && target.Position.B != origin.Position.B && target.Position.C != origin.Position.C)
                {
                    if (target.Position.A == origin.Position.A - origin.Terrain.Width || target.Position.B == origin.Position.B + origin.Terrain.Width) { }
                    else if (target.Position.A == origin.Position.A + origin.Terrain.Width || target.Position.B == origin.Position.B - origin.Terrain.Width) { }
                   else
                        return false;
                }

                if (Math.Max(Math.Max(Math.Abs(target.Position.A - origin.Position.A), Math.Abs(target.Position.B - origin.Position.B)), Math.Abs(target.Position.C - origin.Position.C)) != 5)
                {
                    if (Math.Abs(target.Position.B - origin.Position.B) != origin.Terrain.Width - 5)
                    {
                        if (Math.Abs(target.Position.C - origin.Position.C) != 5)
                            return false;
                    }
                    else if (target.Position.C != origin.Position.C)
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }

    public class UnicornOrderProductionFactory : IActorProductionFactory
    {
        private static Lazy<UnicornOrderProductionFactory> _instance
            = new Lazy<UnicornOrderProductionFactory>(() => new UnicornOrderProductionFactory());
        public static UnicornOrderProductionFactory Instance => _instance.Value;
        private UnicornOrderProductionFactory()
        {
        }

        public Type ResultType => typeof(UnicornOrder);

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
            return new UnicornOrder(owner, point);
        }
    }
}
