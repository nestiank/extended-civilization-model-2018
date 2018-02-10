using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class JediKnight : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("D4B8D80D-4C68-45AD-9EA3-B40CD3377A60");
        public override Guid Guid => ClassGuid;

        public override int MaxAP => 2;

        public override double MaxHP => 30;

        public override double AttackPower => 25;
        public override double DefencePower => 5;

        public override int BattleClassLevel => 3;

        public int SkillDurationTime = 0;

        protected override double CalculateDamage(double originalDamage, Actor opposite, bool isMelee, bool isSkillAttack)
        {
            if (this.SkillDurationTime >= this.Owner.Game.TurnNumber)
            {
                AttackTo(originalDamage, this, opposite.DefencePower, false, true);
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

        public JediKnight(Player owner, Terrain.Point point) : base(owner, point)
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

            public int GetRequiredAP(Terrain.Point? pt)
            {
                if (pt != null)
                    return -1;
                if (!_owner.PlacedPoint.HasValue)
                    return -1;
                if (Owner.Owner.Game.TurnNumber <= LastSkillCalled + 2)
                    return -1;


                return 1;
            }

            public void Act(Terrain.Point? pt)
            {
                if (pt != null)
                    throw new ArgumentException("pt is invalid");
                if (!_owner.PlacedPoint.HasValue)
                    throw new InvalidOperationException("Actor is not placed yet");
                if (Owner.Owner.Game.TurnNumber <= LastSkillCalled + 2)
                    throw new InvalidOperationException("Skill is not turned on");

                _owner.SkillDurationTime = Owner.Owner.Game.TurnNumber + 1;
                LastSkillCalled = Owner.Owner.Game.TurnNumber;
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
        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner, 75, 20, 50, 10);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.Unit == null
                && point.TileBuilding is CityCenter
                && point.TileBuilding.Owner == production.Owner;
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new JediKnight(owner, point);
        }
    }
}
