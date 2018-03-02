using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public class JediKnight : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("D4B8D80D-4C68-45AD-9EA3-B40CD3377A60");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxAP = 2,
            MaxHP = 30,
            AttackPower = 25,
            DefencePower = 5,
            GoldLogistics = 30,
            FullLaborForRepair = 2,
            BattleClassLevel = 3
        };

        public int SkillDurationTime = -1;

        public int SkillFlag = 1;

        protected override double CalculateDamage(double originalDamage, Actor opposite, bool isMelee, bool isSkillAttack)
        {
            if(opposite.BattleClassLevel >= 4 && isSkillAttack)
            {
                return originalDamage;
            }
            else if (this.SkillDurationTime >= this.Owner.Game.TurnNumber && this.SkillFlag > 0)
            {
                AttackTo(originalDamage, opposite, opposite.DefencePower, false, true);
                this.SkillFlag -= 1;
                return 0;
            }
            else if (this.SkillDurationTime >= this.Owner.Game.TurnNumber && this.SkillFlag <= 0)
            {
                this.SkillFlag = 1;
                return 0;
            }
            else
            {
                return originalDamage;
            }
        }

        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public override IReadOnlyList<IActorAction> SpecialActs => _specialActs;
        private readonly IActorAction[] _specialActs = new IActorAction[1];

        public JediKnight(Player owner, Terrain.Point point) : base(owner, Constants, point)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
            _specialActs[0] = new JediKnightAction(this);
        }

        private class JediKnightAction : IActorAction
        {
            private readonly JediKnight _owner;
            public Actor Owner => _owner;

            public bool IsParametered => false;

            public JediKnightAction(JediKnight owner)
            {
                _owner = owner;
            }

            public int LastSkillCalled = -3;

            public double GetRequiredAP(Terrain.Point? pt)
            {
                if (CheckError(pt) != null)
                    return double.NaN;

                return 2;
            }

            private Exception CheckError(Terrain.Point? pt)
            {
                if (pt != null)
                    return new ArgumentException("pt is invalid");
                if (!_owner.PlacedPoint.HasValue)
                    return new InvalidOperationException("Actor is not placed yet");
                if (Owner.Owner.Game.TurnNumber <= LastSkillCalled + 2)
                    return new InvalidOperationException("Skill is not turned on");

                return null;
            }

            public void Act(Terrain.Point? pt)
            {
                if (CheckError(pt) is Exception e)
                    throw e;

                double Ap = GetRequiredAP(pt);
                if (!Owner.CanConsumeAP(Ap))
                    throw new InvalidOperationException("Not enough Ap");


                _owner.SkillDurationTime = Owner.Owner.Game.TurnNumber + 1;
                LastSkillCalled = Owner.Owner.Game.TurnNumber;
                Owner.ConsumeAP(Ap);
            }
        }
    }

    public class JediKnightProductionFactory : ITileObjectProductionFactory
    {
        private static Lazy<JediKnightProductionFactory> _instance
            = new Lazy<JediKnightProductionFactory>(() => new JediKnightProductionFactory());
        public static JediKnightProductionFactory Instance => _instance.Value;
        private JediKnightProductionFactory()
        {
        }

        public Type ResultType => typeof(JediKnight);
        public ActorConstants ActorConstants => JediKnight.Constants;

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
                && point.TileBuilding is CityBase
                && point.TileBuilding.Owner == production.Owner;
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new JediKnight(owner, point);
        }
    }
}
