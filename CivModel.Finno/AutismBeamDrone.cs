using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public class AutismBeamDrone : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("B1637348-A97F-4D7F-B160-B82E4695F2C3");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxAP = 2,
            MaxHP = 35,
            AttackPower = 20,
            DefencePower = 5,
            GoldLogistics = 30,
            FullLaborLogistics = 2,
            BattleClassLevel = 3
        };

        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public override IReadOnlyList<IActorAction> SpecialActs => _specialActs;
        private readonly IActorAction[] _specialActs = new IActorAction[1];

        public AutismBeamDrone(Player owner, Terrain.Point point) : base(owner, Constants, point)
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

            public double GetRequiredAP(Terrain.Point? pt)
            {
                if (CheckError(pt) != null)
                    return -1;

                return 2;
            }

            public void Act(Terrain.Point? pt)
            {
                if (CheckError(pt) is Exception e)
                    throw e;

                double Ap = GetRequiredAP(pt);
                if (!_owner.CanConsumeAP(Ap))
                    throw new InvalidOperationException("Not enough Ap");


                new ControlHijackEffect(pt.Value.Unit, Owner.Owner).EffectOn();

                LastSkillCalled = Owner.Owner.Game.TurnNumber;
                Owner.ConsumeAP(Ap);
            }

            private Exception CheckError(Terrain.Point? pt)
            {
                if (!_owner.PlacedPoint.HasValue)
                    return new InvalidOperationException("Actor is not placed yet");
                if (pt == null)
                    return new ArgumentNullException(nameof(pt));
                if (Owner.Owner.Game.TurnNumber <= LastSkillCalled + 2)
                    return new InvalidOperationException("Skill is not turned on");

                if (pt.Value.Unit is Unit unit && unit.Owner != Owner.Owner)
                {
                    if (pt.Value.TileBuilding != null)
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

    class ControlHijackEffect : Effect
    {
        private readonly Player _hijacker;
        private Player _hijackee;

        private const int _stealTurn = 2;
        private const int _stunTurn = 1;

        private Action DoOff;

        public ControlHijackEffect(Actor target, Player hijacker)
            : base(target, _stealTurn + _stunTurn, EffectTag.Ownership)
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

        public override void PostTurn()
        {
            base.PostTurn();
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

    public class AutismBeamDroneFactory : ITileObjectProductionFactory
    {
        private static Lazy<AutismBeamDroneFactory> _instance
            = new Lazy<AutismBeamDroneFactory>(() => new AutismBeamDroneFactory());
        public static AutismBeamDroneFactory Instance => _instance.Value;
        private AutismBeamDroneFactory()
        {
        }

        public ActorConstants ActorConstants => AutismBeamDrone.Constants;

        public double TotalLaborCost => 50;
        public double LaborCapacityPerTurn => 20;
        public double TotalGoldCost => 75;
        public double GoldCapacityPerTurn => 11;

        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.Unit == null
                && point.TileBuilding is CivModel.Common.CityCenter
                && point.TileBuilding.Owner == production.Owner;
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new AutismBeamDrone(owner, point);
        }
    }
}
