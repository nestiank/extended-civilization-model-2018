using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class ProtoNinja : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("87710C32-94A3-4A9D-92ED-8BB29EE2B475");
        public override Guid Guid => ClassGuid;

        public override int MaxAP => 2;

        public override double MaxHP => 35;

        public override double AttackPower => 20;
        public override double DefencePower => 5;

        public override int BattleClassLevel => 3;

        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public override IReadOnlyList<IActorAction> SpecialActs => _specialActs;
        private readonly IActorAction[] _specialActs = new IActorAction[1];

        public ProtoNinja(Player owner, Terrain.Point point) : base(owner, point)
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

            public int GetRequiredAP(Terrain.Point? pt)
            {
                if (pt == null)
                    return -1;
                if (!_owner.PlacedPoint.HasValue)
                    return -1;
                if (Owner.Owner.Game.TurnNumber < LastSkillCalled + 3)
                    return -1;
                if (pt.Value.Unit == null)
                    return -1;
                if (Math.Max(Math.Max(Math.Abs(pt.Value.Position.A - Owner.PlacedPoint.Value.Position.A), Math.Abs(pt.Value.Position.B - Owner.PlacedPoint.Value.Position.B)), Math.Abs(pt.Value.Position.C - Owner.PlacedPoint.Value.Position.C)) > 2)
                    return -1;
                if (pt.Value.Unit.Owner == Owner.Owner)
                    return -1;
                if (pt.Value.Unit.BattleClassLevel > 3)
                    return -1;

                return 1;
            }

            public void Act(Terrain.Point? pt)
            {
                if (pt != null)
                    throw new ArgumentException("pt is invalid");
                if (!_owner.PlacedPoint.HasValue)
                    throw new InvalidOperationException("Actor is not placed yet");
                if (Owner.Owner.Game.TurnNumber < LastSkillCalled + 3)
                    throw new InvalidOperationException("Skill is not turned on");
                if (pt.Value.Unit == null)
                    throw new InvalidOperationException("There is no target");
                if (Math.Max(Math.Max(Math.Abs(pt.Value.Position.A - Owner.PlacedPoint.Value.Position.A), Math.Abs(pt.Value.Position.B - Owner.PlacedPoint.Value.Position.B)), Math.Abs(pt.Value.Position.C - Owner.PlacedPoint.Value.Position.C)) > 2)
                    throw new InvalidOperationException("Too far to attack");
                if (pt.Value.Unit.Owner == Owner.Owner)
                    throw new InvalidOperationException("The Unit is friendly");
                if (pt.Value.Unit.BattleClassLevel > 3)
                    throw new InvalidOperationException("The Unit's ClassLevel is more then limit");

                Owner.AttackTo(pt.Value.Unit.MaxHP, pt.Value.Unit, 0, true, true);

                if (pt.Value.Unit == null && Owner != null)
                {
                    Owner.PlacedPoint = pt;
                }
            }
        }

        public class ProtoNinjaProductionFactory : ITileObjectProductionFactory
        {
            private static Lazy<ProtoNinjaProductionFactory> _instance
                = new Lazy<ProtoNinjaProductionFactory>(() => new ProtoNinjaProductionFactory());
            public static ProtoNinjaProductionFactory Instance => _instance.Value;
            private ProtoNinjaProductionFactory()
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
                return new ProtoNinja(owner, point);
            }
        }
    }
}

