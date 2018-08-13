using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public class LEOSpaceArmada : Unit
    {
        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public override IReadOnlyList<IActorAction> SpecialActs => _specialActs;
        private readonly IActorAction[] _specialActs = new IActorAction[1];

        public LEOSpaceArmada(Player owner, Terrain.Point point) : base(owner, typeof(LEOSpaceArmada), point)
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

            public ActionPoint GetRequiredAP(Terrain.Point origin, Terrain.Point? target)
            {
                if (CheckError(origin, target) != null)
                    return double.NaN;

                return 2;
            }

            private Exception CheckError(Terrain.Point origin, Terrain.Point? target)
            {
                if (target == null)
                    return new ArgumentNullException(nameof(target));
                if (Owner.Owner.Game.TurnNumber <= LastSkillCalled + 2)
                    return new InvalidOperationException("Skill is not turned on");
                if (target.Value.Unit == null && target.Value.TileBuilding == null)
                    return new InvalidOperationException("There is no target");

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

                if(target.Value.Unit != null)
                    Owner.AttackTo(Owner.AttackPower * 2, target.Value.Unit, target.Value.Unit.DefencePower,false, true);

                if(target.Value.TileBuilding != null)
                    Owner.AttackTo(Owner.AttackPower * 2, target.Value.TileBuilding, target.Value.TileBuilding.DefencePower, false, true);
                LastSkillCalled = Owner.Owner.Game.TurnNumber;
                Owner.ConsumeAP(Ap);
            }
        }
    }

    public class LEOSpaceArmadaProductionFactory : IActorProductionFactory
    {
        private static Lazy<LEOSpaceArmadaProductionFactory> _instance
            = new Lazy<LEOSpaceArmadaProductionFactory>(() => new LEOSpaceArmadaProductionFactory());
        public static LEOSpaceArmadaProductionFactory Instance => _instance.Value;
        private LEOSpaceArmadaProductionFactory()
        {
        }

        public Type ResultType => typeof(LEOSpaceArmada);

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
            return new LEOSpaceArmada(owner,point);
        }
    }
}
