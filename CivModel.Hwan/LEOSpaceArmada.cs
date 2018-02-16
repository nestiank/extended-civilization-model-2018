using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public class LEOSpaceArmada : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("A41789C8-823A-4B13-BDE2-3171751FC8BF");
        public override Guid Guid => ClassGuid;

        public override double MaxAP => 2;

        public override double MaxHP => 50;

        public override double AttackPower => 15;
        public override double DefencePower => 5;

        public override double GoldLogistics => 1;
        public override double FullLaborLogicstics => 1;

        public override int BattleClassLevel => 2;

        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public override IReadOnlyList<IActorAction> SpecialActs => _specialActs;
        private readonly IActorAction[] _specialActs = new IActorAction[1];

        public LEOSpaceArmada(Player owner, Terrain.Point point) : base(owner, point)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
            _specialActs[0] = new LEOSpaceArmadaAction(this);
        }


        private class LEOSpaceArmadaAction : IActorAction
        {
            private readonly LEOSpaceArmada _owner;
            public Actor Owner => _owner;

            public bool IsParametered => true;

            public LEOSpaceArmadaAction(LEOSpaceArmada owner)
            {
                _owner = owner;
            }

            public int LastSkillCalled = -3;

            public int GetRequiredAP(Terrain.Point? pt)
            {
                if (pt == null)
                    return -1;
                if (!_owner.PlacedPoint.HasValue)
                    return -1;
                if (Owner.Owner.Game.TurnNumber <= LastSkillCalled + 2)
                    return -1;
                if (pt.Value.Unit == null)
                    return -1;

                return 1;
            }

            public void Act(Terrain.Point? pt)
            {
                if (pt == null)
                    throw new ArgumentException("pt is invalid");
                if (!_owner.PlacedPoint.HasValue)
                    throw new InvalidOperationException("Actor is not placed yet");
                if (Owner.Owner.Game.TurnNumber <= LastSkillCalled + 2)
                    throw new InvalidOperationException("Skill is not turned on");
                if (pt.Value.Unit == null)
                    throw new InvalidOperationException("There is no target");

                int Ap = GetRequiredAP(pt);
                if (!Owner.CanConsumeAP(Ap))
                    throw new InvalidOperationException("Not enough Ap");

                Owner.AttackTo(Owner.AttackPower * 2, pt.Value.Unit, pt.Value.Unit.DefencePower,false, true);
                LastSkillCalled = Owner.Owner.Game.TurnNumber;
                Owner.ConsumeAP(Ap);
            }
        }
    }

    public class LEOSpaceArmadaProductionFactory : ITileObjectProductionFactory
    {
        private static Lazy<LEOSpaceArmadaProductionFactory> _instance
            = new Lazy<LEOSpaceArmadaProductionFactory>(() => new LEOSpaceArmadaProductionFactory());
        public static LEOSpaceArmadaProductionFactory Instance => _instance.Value;
        private LEOSpaceArmadaProductionFactory()
        {
        }
        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner, 50, 15, 20, 4);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.Unit == null
                && point.TileBuilding is CivModel.Common.CityCenter
                && point.TileBuilding.Owner == production.Owner;
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new LEOSpaceArmada(owner,point);
        }
    }
}
