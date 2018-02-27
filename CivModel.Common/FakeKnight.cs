using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class FakeKnight : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("8209396E-45E3-441C-879F-29EFE9EDC23C");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants {
            MaxAP = 4,
            MaxHP = 30,
            AttackPower = 25,
            DefencePower = 5,
            GoldLogistics = 5,
            LaborLogistics = 2.5,
            FullLaborForRepair = 0.5,
            BattleClassLevel = 2
        };

        public override IActorAction HoldingAttackAct => _holdingAttackAct;
        private readonly IActorAction _holdingAttackAct;

        public override IActorAction MovingAttackAct => _movingAttackAct;
        private readonly IActorAction _movingAttackAct;

        public override IReadOnlyList<IActorAction> SpecialActs => _specialActs;
        private readonly IActorAction[] _specialActs = new IActorAction[1];

        public FakeKnight(Player owner, Terrain.Point point) : base(owner, Constants, point)
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

            public double GetRequiredAP(Terrain.Point? pt)
            {
                if (CheckError(pt) != null)
                    return double.NaN;

                return 1;
            }

            public void Act(Terrain.Point? pt)
            {
                if (CheckError(pt) is Exception e)
                    throw e;

                new ControlHijackEffect(pt.Value.Unit, Owner.Owner).EffectOn();
            }

            private Exception CheckError(Terrain.Point? pt)
            {
                if (!_owner.PlacedPoint.HasValue)
                    return new InvalidOperationException("Actor is not placed yet");
                if (pt == null)
                    return new ArgumentNullException(nameof(pt));

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

    public class FakeKnightProductionFactory : IActorProductionFactory
    {
        private static Lazy<FakeKnightProductionFactory> _instance
            = new Lazy<FakeKnightProductionFactory>(() => new FakeKnightProductionFactory());
        public static FakeKnightProductionFactory Instance => _instance.Value;
        private FakeKnightProductionFactory()
        {
        }

        public ActorConstants ActorConstants => FakeKnight.Constants;

        public double TotalLaborCost => 7.5;
        public double LaborCapacityPerTurn => 3;
        public double TotalGoldCost => 7.5;
        public double GoldCapacityPerTurn => 3;

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
