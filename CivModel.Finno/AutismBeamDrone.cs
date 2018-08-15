using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public class AutismBeamDrone : Unit
    {
        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public override IReadOnlyList<IActorAction> SpecialActs => _specialActs;
        private readonly IActorAction[] _specialActs = new IActorAction[1];

        public AutismBeamDrone(Player owner, Terrain.Point point) : base(owner, typeof(AutismBeamDrone), point)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
            _specialActs[0] = new AutismBeamDroneAction(this);
        }

        private class AutismBeamDroneAction : IActorAction
        {
            public Actor Owner => _owner;
            private readonly AutismBeamDrone _owner;

            public bool IsParametered => true;

            public AutismBeamDroneAction(AutismBeamDrone owner)
            {
                _owner = owner;
            }

            public int LastSkillCalled = -3;

            public ActionPoint GetRequiredAP(Terrain.Point origin, Terrain.Point? target)
            {
                if (CheckError(origin, target) != null)
                    return double.NaN;

                return 2;
            }

            private bool IsInDistance(Terrain.Point origin, Terrain.Point target)
            {
                int A = origin.Position.A;
                int B = origin.Position.B;
                int C = origin.Position.C;
                int Width = origin.Terrain.Width;

                if (Math.Max(Math.Max(Math.Abs(target.Position.A - A), Math.Abs(target.Position.B - B)), Math.Abs(target.Position.C - C)) > 3)
                {
                    if (target.Position.B > B) // pt가 맵 오른쪽
                    {
                        if (Math.Max(Math.Max(Math.Abs(target.Position.B - Width - B), Math.Abs(target.Position.A + Width - A)), Math.Abs(target.Position.C - C)) > 3)
                            return false;
                    }
                    else //pt가 맵 왼쪽
                    {
                        if (Math.Max(Math.Max(Math.Abs(target.Position.B + Width - B), Math.Abs(target.Position.A - Width - A)), Math.Abs(target.Position.C - C)) > 3)
                            return false;
                    }
                }
                return true;
            }

            public void Act(Terrain.Point? pt)
            {
                if (!_owner.PlacedPoint.HasValue)
                    throw new InvalidOperationException("Actor is not placed yet");
                if (CheckError(_owner.PlacedPoint.Value, pt) is Exception e)
                    throw e;

                ActionPoint Ap = GetRequiredAP(_owner.PlacedPoint.Value, pt);
                if (!_owner.CanConsumeAP(Ap))
                    throw new InvalidOperationException("Not enough Ap");


                new ControlHijackEffect(pt.Value.Unit, Owner.Owner).EffectOn();

                LastSkillCalled = Owner.Owner.Game.TurnNumber;
                Owner.ConsumeAP(Ap);
            }

            private Exception CheckError(Terrain.Point origin, Terrain.Point? target)
            {
                if (target == null)
                    return new ArgumentNullException(nameof(target));
                if (Owner.Owner.Game.TurnNumber <= LastSkillCalled + 2)
                    return new InvalidOperationException("Skill is not turned on");
                if (!this.IsInDistance(origin, target.Value))
                    return new InvalidOperationException("Too Far to Use Autism Beam");

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

    class ControlHijackEffect : ActorEffect
    {
        private readonly Player _hijacker;
        private Player _hijackee;

        private const int _stealTurn = 2;
        private const int _stunTurn = 1;

        private Action DoOff;

        public ControlHijackEffect(Actor target, Player hijacker)
            : base(target, _stealTurn + _stunTurn)
        {
            _hijacker = hijacker;
        }

        protected override void OnEffectOn()
        {
            Steal();
        }

        protected override void OnEffectOff()
        {
            DoOff();
        }

        protected override void OnTargetDestroy()
        {
        }

        protected override void FixedPostTurn()
        {
            base.FixedPostTurn();
            if (Enabled && LeftTurn == _stunTurn)
            {
                DoOff();
                if (Target != null)
                    Stun();
            }
        }

        private void Steal()
        {
            _hijackee = Target.Owner;
            Target.Owner = _hijacker;
            DoOff = () => {
                if (Target.PlacedPoint?.TileBuilding == null)
                {
                    Target.Owner = _hijackee;
                    Target.SkipFlag = false;
                }
                else
                {
                    // if Target is on TileBuilding of hijacker, Ownership cannot be changed
                    // Target must be moved to another position or destroyed.
                    Target.Destroy();
                }
            };
        }

        private void Stun()
        {
            Target.IsControllable = false;
            DoOff = () => Target.IsControllable = true;
        }
    }

    public class AutismBeamDroneFactory : IActorProductionFactory
    {
        private static Lazy<AutismBeamDroneFactory> _instance
            = new Lazy<AutismBeamDroneFactory>(() => new AutismBeamDroneFactory());
        public static AutismBeamDroneFactory Instance => _instance.Value;
        private AutismBeamDroneFactory()
        {
        }

        public Type ResultType => typeof(AutismBeamDrone);

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
            return new AutismBeamDrone(owner, point);
        }
    }
}
