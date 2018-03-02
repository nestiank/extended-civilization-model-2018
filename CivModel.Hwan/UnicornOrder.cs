using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public class UnicornOrder : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("06128735-B62C-4A40-AC4E-35F26C49A6EC");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxAP = 2,
            MaxHP = 50,
            AttackPower = 17,
            DefencePower = 5,
            GoldLogistics = 20,
            FullLaborForRepair = 2,
            BattleClassLevel = 2
        };

        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public override IReadOnlyList<IActorAction> SpecialActs => _specialActs;
        private readonly IActorAction[] _specialActs = new IActorAction[1];

        public UnicornOrder(Player owner, Terrain.Point point) : base(owner, Constants, point)
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

                double Ap = GetRequiredAP(pt);
                if (!Owner.CanConsumeAP(Ap))
                    throw new InvalidOperationException("Not enough Ap");


                Owner.PlacedPoint = pt;

                LastSkillCalled = Owner.Owner.Game.TurnNumber;
                Owner.ConsumeAP(Ap);
            }

            private Exception CheckError(Terrain.Point? pt)
            {
                if (pt == null)
                    return new ArgumentException("pt is invalid");
                if (!_owner.PlacedPoint.HasValue)
                    return new InvalidOperationException("Actor is not placed yet");
                if (Owner.Owner.Game.TurnNumber <= LastSkillCalled + 1)
                    return new InvalidOperationException("Skill is not turned on");
                if (pt.Value.Unit != null)
                    return new InvalidOperationException("Can't go that way");
                if (!this.DirectionCheck(pt))
                    return new InvalidOperationException("Can't go that way");

                return null;
            }

           private bool DirectionCheck(Terrain.Point? pt)
            {
                if (pt.Value.Position.A != Owner.PlacedPoint.Value.Position.A && pt.Value.Position.B != Owner.PlacedPoint.Value.Position.B && pt.Value.Position.C != Owner.PlacedPoint.Value.Position.C)
                    return false;
                if (Math.Max(Math.Max(Math.Abs(pt.Value.Position.A - Owner.PlacedPoint.Value.Position.A), Math.Abs(pt.Value.Position.B - Owner.PlacedPoint.Value.Position.B)), Math.Abs(pt.Value.Position.C - Owner.PlacedPoint.Value.Position.C)) != 5)
                {
                    if (Math.Abs(pt.Value.Position.B - Owner.PlacedPoint.Value.Position.B) !=  Owner.PlacedPoint.Value.Terrain.Width - 5)
                        return false;
                }


                return true;
            }
        }
    }

    public class UnicornOrderProductionFactory : ITileObjectProductionFactory
    {
        private static Lazy<UnicornOrderProductionFactory> _instance
            = new Lazy<UnicornOrderProductionFactory>(() => new UnicornOrderProductionFactory());
        public static UnicornOrderProductionFactory Instance => _instance.Value;
        private UnicornOrderProductionFactory()
        {
        }

        public Type ResultType => typeof(UnicornOrder);
        public ActorConstants ActorConstants => UnicornOrder.Constants;

        public double TotalLaborCost => 30;
        public double LaborCapacityPerTurn => 15;
        public double TotalGoldCost => 50;
        public double GoldCapacityPerTurn => 10;

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
            return new UnicornOrder(owner, point);
        }
    }
}
