using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public class ProtoNinja : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("87710C32-94A3-4A9D-92ED-8BB29EE2B475");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxAP = 2,
            MaxHP = 35,
            AttackPower = 20,
            DefencePower = 5,
            GoldLogistics = 30,
            FullLaborForRepair = 2,
            BattleClassLevel = 3
        };

        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public override IReadOnlyList<IActorAction> SpecialActs => _specialActs;
        private readonly IActorAction[] _specialActs = new IActorAction[1];

        public ProtoNinja(Player owner, Terrain.Point point) : base(owner, Constants, point)
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

            public double GetRequiredAP(Terrain.Point? pt)
            {
                if (CheckError(pt) != null)
                    return double.NaN;

                return 1;
            }

            private Exception CheckError(Terrain.Point? pt)
            {
                if (pt == null)
                    return new ArgumentException("pt is invalid");
                if (!_owner.PlacedPoint.HasValue)
                    return new InvalidOperationException("Actor is not placed yet");
                if (Owner.Owner.Game.TurnNumber <= LastSkillCalled + 2)
                    return new InvalidOperationException("Skill is not turned on");
                if (pt.Value.Unit == null)
                    return new InvalidOperationException("There is no target");
                if (Math.Max(Math.Max(Math.Abs(pt.Value.Position.A - Owner.PlacedPoint.Value.Position.A), Math.Abs(pt.Value.Position.B - Owner.PlacedPoint.Value.Position.B)), Math.Abs(pt.Value.Position.C - Owner.PlacedPoint.Value.Position.C)) > 2)
                    return new InvalidOperationException("Too far to attack");
                if (pt.Value.Unit.Owner == Owner.Owner)
                    return new InvalidOperationException("The Unit is friendly");
                if (pt.Value.TileBuilding != null)
                    return new InvalidOperationException("The Unit is in Building");
                if (pt.Value.Unit.BattleClassLevel > 3)
                    return new InvalidOperationException("The Unit's ClassLevel is more then limit");

                return null;
            }

            public void Act(Terrain.Point? pt)
            {
                if (CheckError(pt) is Exception e)
                    throw e;

                double Ap = GetRequiredAP(pt);
                if (!Owner.CanConsumeAP(Ap))
                    throw new InvalidOperationException("Not enough Ap");


                Owner.AttackTo(pt.Value.Unit.MaxHP, pt.Value.Unit, 0, true, true);

                if (pt.Value.Unit == null && Owner != null)
                {
                    Owner.PlacedPoint = pt;
                }
                LastSkillCalled = Owner.Owner.Game.TurnNumber;
                Owner.ConsumeAP(Ap);
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

        public Type ResultType => typeof(ProtoNinja);
        public ActorConstants ActorConstants => ProtoNinja.Constants;

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
            return new ProtoNinja(owner, point);
        }
    }
}

