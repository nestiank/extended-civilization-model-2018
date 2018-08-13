using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public class AncientSorcerer : Unit
    {
        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public override IReadOnlyList<IActorAction> SpecialActs => _specialActs;
        private readonly IActorAction[] _specialActs = new IActorAction[1];

        public AncientSorcerer(Player owner, Terrain.Point point) : base(owner, typeof(AncientSorcerer), point)
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


            public ActionPoint GetRequiredAP(Terrain.Point origin, Terrain.Point? target)
            {
                if (CheckError(origin, target) != null)
                    return double.NaN;

                return 1;
            }

            private bool IsInDistance(Terrain.Point origin, Terrain.Point target)
            {
                int A = origin.Position.A;
                int B = origin.Position.B;
                int C = origin.Position.C;
                int Width = origin.Terrain.Width;

                if (Math.Max(Math.Max(Math.Abs(target.Position.A - A), Math.Abs(target.Position.B - B)), Math.Abs(target.Position.C - C)) > 2)
                {
                    if(target.Position.B > B) // pt가 맵 오른쪽
                    {
                        if (Math.Max(Math.Max(Math.Abs(target.Position.B - Width - B), Math.Abs(target.Position.A + Width - A)),Math.Abs(target.Position.C - C)) > 2)
                            return false;
                    }
                    else //pt가 맵 왼쪽
                    {
                        if (Math.Max(Math.Max(Math.Abs(target.Position.B + Width - B), Math.Abs(target.Position.A - Width - A)), Math.Abs(target.Position.C - C)) > 2)
                            return false;
                    }
                }
                return true;
            }

            private Exception CheckError(Terrain.Point origin, Terrain.Point? target)
            {
                if (target == null)
                    return new ArgumentNullException(nameof(target));
                if (target.Value.Unit == null)
                    return new InvalidOperationException("There is no target");
                if (target.Value.Unit.Owner != Owner.Owner)
                    return new InvalidOperationException("The Unit is hostile");
                if (target.Value.Unit.MaxHP == target.Value.Unit.RemainHP)
                    return new InvalidOperationException("The Unit Has Full HP"); //만피 회복 불가
                if (!this.IsInDistance(origin, target.Value))
                    return new InvalidOperationException("Too Far to Heal");

                return null;
            }

            public void Act(Terrain.Point? pt)
            {
                if (!_owner.PlacedPoint.HasValue)
                    throw new InvalidOperationException("Actor is not placed yet");
                if (CheckError(_owner.PlacedPoint.Value, pt) is Exception e)
                    throw e;

                ActionPoint Ap = GetRequiredAP(_owner.PlacedPoint.Value, pt);
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
