using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public class ElephantCavalry : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("8AC71759-3B58-4637-9F09-4F483EB0F4B8");
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

        public ElephantCavalry(Player owner, Terrain.Point point) : base(owner, Constants, point)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
            _specialActs[0] = new ElephantCavalryAction(this);
        }

        private class ElephantCavalryAction : IActorAction
        {
            private readonly ElephantCavalry _owner;
            public Actor Owner => _owner;

            public bool IsParametered => true;

            public ElephantCavalryAction(ElephantCavalry owner)
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

            private void Stamping(int A,int B, int C)
            {
                if ((Owner.PlacedPoint.Value.Terrain.GetPoint(A, B, C)).Unit != null)
                {
                    if ((Owner.PlacedPoint.Value.Terrain.GetPoint(A, B, C)).Unit.Owner != Owner.Owner)
                    {
                        double Damage = (Owner.PlacedPoint.Value.Terrain.GetPoint(A, B, C)).Unit.MaxHP * 0.2;
                        Owner.AttackTo(Damage, (Owner.PlacedPoint.Value.Terrain.GetPoint(A, B, C)).Unit, 0, false, true);
                    }
                }
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
                if (pt.Value.Position.A != Owner.PlacedPoint.Value.Position.A && pt.Value.Position.B != Owner.PlacedPoint.Value.Position.B && pt.Value.Position.C != Owner.PlacedPoint.Value.Position.C)
                    return new InvalidOperationException("Can't go that way");
                if (Math.Max(Math.Max(Math.Abs(pt.Value.Position.A - Owner.PlacedPoint.Value.Position.A), Math.Abs(pt.Value.Position.B - Owner.PlacedPoint.Value.Position.B)), Math.Abs(pt.Value.Position.C - Owner.PlacedPoint.Value.Position.C)) != 3)
                    return new InvalidOperationException("Can't go that way");


                return null;
            }

            public void Act(Terrain.Point? pt)
            {
                if (CheckError(pt) is Exception e)
                    throw e;

                double Ap = GetRequiredAP(pt);
                if (!Owner.CanConsumeAP(Ap))
                    throw new InvalidOperationException("Not enough Ap");


                Owner.PlacedPoint = pt;

                int A = Owner.PlacedPoint.Value.Position.A;
                int B = Owner.PlacedPoint.Value.Position.B;
                int C = Owner.PlacedPoint.Value.Position.C;

                if (pt.Value.Position.A == Owner.PlacedPoint.Value.Position.A)
                {
                    if (pt.Value.Position.B < Owner.PlacedPoint.Value.Position.B)
                    {
                        this.Stamping(A, B - 1, C + 1);
                        this.Stamping(A, B - 2, C + 2);
                    }

                    if (pt.Value.Position.B > Owner.PlacedPoint.Value.Position.B)
                    {
                        this.Stamping(A, B + 1, C - 1);
                        this.Stamping(A, B + 2, C - 2);
                    }
                }

                if (pt.Value.Position.B == Owner.PlacedPoint.Value.Position.B)
                {
                    if (pt.Value.Position.A < Owner.PlacedPoint.Value.Position.A)
                    {
                        this.Stamping(A - 1, B, C + 1);
                        this.Stamping(A - 2, B, C + 2);
                    }

                    if (pt.Value.Position.A > Owner.PlacedPoint.Value.Position.A)
                    {
                        this.Stamping(A + 1, B, C - 1);
                        this.Stamping(A + 2, B, C - 2);
                    }
                }

                if (pt.Value.Position.C == Owner.PlacedPoint.Value.Position.C)
                {
                    if (pt.Value.Position.A < Owner.PlacedPoint.Value.Position.A)
                    {
                        this.Stamping(A - 1, B + 1, C);
                        this.Stamping(A - 2, B + 2, C);
                    }

                    if (pt.Value.Position.A > Owner.PlacedPoint.Value.Position.A)
                    {
                        this.Stamping(A + 1, B - 1, C);
                        this.Stamping(A + 2, B - 2, C);
                    }
                }

                LastSkillCalled = Owner.Owner.Game.TurnNumber;
                Owner.ConsumeAP(Ap);
            }
        }
    }

    public class ElephantCavalryProductionFactory : ITileObjectProductionFactory
    {
        private static Lazy<ElephantCavalryProductionFactory> _instance
            = new Lazy<ElephantCavalryProductionFactory>(() => new ElephantCavalryProductionFactory());
        public static ElephantCavalryProductionFactory Instance => _instance.Value;
        private ElephantCavalryProductionFactory()
        {
        }

        public ActorConstants ActorConstants => ElephantCavalry.Constants;

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
            return new ElephantCavalry(owner, point);
        }
    }
}
