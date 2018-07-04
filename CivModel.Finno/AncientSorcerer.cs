using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public class AncientSorcerer : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("C49FB444-C530-41B8-9D61-A9CF4443A4D6");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxAP = 2,
            MaxHP = 50,
            AttackPower = 15,
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

        public AncientSorcerer(Player owner, Terrain.Point point) : base(owner, Constants, point)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
            _specialActs[0] = new AncientSorcererAction(this);
        }

        private class AncientSorcererAction : IActorAction
        {
            private readonly AncientSorcerer _owner;
            public Actor Owner => _owner;

            public bool IsParametered => true;

            public AncientSorcererAction(AncientSorcerer owner)
            {
                _owner = owner;
            }


            public ActionPoint GetRequiredAP(Terrain.Point? pt)
            {
                if (CheckError(pt) != null)
                    return double.NaN;

                return 1;
            }

            private bool IsInDistance(Terrain.Point? pt)
            {
                int A = Owner.PlacedPoint.Value.Position.A;
                int B = Owner.PlacedPoint.Value.Position.B;
                int C = Owner.PlacedPoint.Value.Position.C;
                int Width = Owner.PlacedPoint.Value.Terrain.Width;

                if (Math.Max(Math.Max(Math.Abs(pt.Value.Position.A - A), Math.Abs(pt.Value.Position.B - B)), Math.Abs(pt.Value.Position.C - C)) > 2)
                {
                    if(pt.Value.Position.B > B) // pt가 맵 오른쪽
                    {
                        if (Math.Max(Math.Max(Math.Abs(pt.Value.Position.B - Width - B), Math.Abs(pt.Value.Position.A + Width - A)),Math.Abs(pt.Value.Position.C - C)) > 2)
                            return false;
                    }
                    else //pt가 맵 왼쪽
                    {
                        if (Math.Max(Math.Max(Math.Abs(pt.Value.Position.B + Width - B), Math.Abs(pt.Value.Position.A - Width - A)), Math.Abs(pt.Value.Position.C - C)) > 2)
                            return false;
                    }
                }
                return true;
            }

            private Exception CheckError(Terrain.Point? pt)
            {
                if (pt == null)
                    return new ArgumentException("pt is invalid");
                if (!_owner.PlacedPoint.HasValue)
                    return new InvalidOperationException("Actor is not placed yet");
                if (pt.Value.Unit == null)
                    return new InvalidOperationException("There is no target");
                if (pt.Value.Unit.Owner != Owner.Owner)
                    return new InvalidOperationException("The Unit is hostile");
                if(pt.Value.Unit.MaxHP == pt.Value.Unit.RemainHP)
                    return new InvalidOperationException("The Unit Has Full HP"); //만피 회복 불가
                if (!this.IsInDistance(pt))
                    return new InvalidOperationException("Too Far to Heal");

                return null;
            }

            public void Act(Terrain.Point? pt)
            {
                if (CheckError(pt) is Exception e)
                    throw e;

                ActionPoint Ap = GetRequiredAP(pt);
                if (!Owner.CanConsumeAP(Ap))
                    throw new InvalidOperationException("Not enough Ap");

                Unit Patient = pt.Value.Unit;
                double AmountOfHeal = 0;
                double NeedHeal = Patient.MaxHP - Patient.RemainHP;

                AmountOfHeal = Math.Min(10, Owner.RemainHP);
                AmountOfHeal = Math.Min(AmountOfHeal, NeedHeal);

                pt.Value.Unit.Heal(AmountOfHeal);

                Owner.RemainHP = Owner.RemainHP - AmountOfHeal;
                if(Owner.RemainHP <= 0)
                {
                    return;
                }
                Owner.ConsumeAP(Ap);

            }
        }
    }

    public class AncientSorcererProductionFactory : IActorProductionFactory
    {
        private static Lazy<AncientSorcererProductionFactory> _instance
            = new Lazy<AncientSorcererProductionFactory>(() => new AncientSorcererProductionFactory());
        public static AncientSorcererProductionFactory Instance => _instance.Value;
        private AncientSorcererProductionFactory()
        {
        }

        public Type ResultType => typeof(AncientSorcerer);
        public ActorConstants ActorConstants => AncientSorcerer.Constants;

        public double TotalLaborCost => 35;
        public double LaborCapacityPerTurn => 15;
        public double TotalGoldCost => 50;
        public double GoldCapacityPerTurn => 20;

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
            return new AncientSorcerer(owner, point);
        }
    }
}
