using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public sealed class HwanEmpireCity : CityBase
    {
        public static Guid ClassGuid { get; } = new Guid("D0A84907-885A-44C2-8E4C-077744E1E0C3");
        public override Guid Guid => ClassGuid;

        public override double MaxHP => 500;

        public override void PostTurn()
        {
            this.RemainHP = Math.Min(500, (this.RemainHP + 20));
        }

        public override IReadOnlyList<IActorAction> SpecialActs => _specialActs;
        private readonly IActorAction[] _specialActs = new IActorAction[1];

        public HwanEmpireCity(Player player, Terrain.Point point) : base(player, point)
        {
            _specialActs[0] = new HwanEmpireCityAction(this);
        }

        private class HwanEmpireCityAction : IActorAction
        {
            private readonly HwanEmpireCity _owner;
            public Actor Owner => _owner;

            public bool IsParametered => false;

            public HwanEmpireCityAction(HwanEmpireCity owner)
            {
                _owner = owner;
            }

            public int GetRequiredAP(Terrain.Point? pt)
            {
                if (pt != null)
                    return -1;
                if (!_owner.PlacedPoint.HasValue)
                    return -1;

                return 0;
            }

            public void Act(Terrain.Point? pt)
            {
                if (pt != null)
                    throw new ArgumentException("pt is invalid");
                if (!_owner.PlacedPoint.HasValue)
                    throw new InvalidOperationException("Actor is not placed yet");

                int A = Owner.PlacedPoint.Value.Position.A;
                int B = Owner.PlacedPoint.Value.Position.B;
                int C = Owner.PlacedPoint.Value.Position.C;

                RealAction(A + 1, B - 1, C);
                RealAction(A + 1, B, C - 1);
                RealAction(A, B + 1, C - 1);
                RealAction(A - 1, B + 1, C);
                RealAction(A - 1, B, C + 1);
                RealAction(A, B - 1, C + 1);

                RealAction(A + 2, B - 2, C);
                RealAction(A + 2, B - 1, C - 1);
                RealAction(A + 2, B, C - 2);
                RealAction(A + 1, B + 1, C - 2);
                RealAction(A, B + 2, C - 2);
                RealAction(A - 1, B + 2, C - 1);
                RealAction(A - 2, B + 2, C);
                RealAction(A - 2, B + 1, C + 1);
                RealAction(A - 2, B, C + 2);
                RealAction(A - 1, B - 1, C + 2);
                RealAction(A, B - 2, C + 2);
                RealAction(A + 1, B - 2, C + 1);
            }

            public void RealAction(int A,int B,int C)
            {
                if (0 <= B + (C + Math.Sign(C)) / 2 && B + (C + Math.Sign(C)) / 2 < Owner.PlacedPoint.Value.Terrain.Width && 0 <= C && C < Owner.PlacedPoint.Value.Terrain.Height)
                {
                    if ((Owner.PlacedPoint.Value.Terrain.GetPoint(A, B, C)).Unit != null)
                    {
                        if ((Owner.PlacedPoint.Value.Terrain.GetPoint(A, B, C)).Unit.Owner != Owner.Owner)
                        {
                            Owner.AttackTo(30, (Owner.PlacedPoint.Value.Terrain.GetPoint(A, B, C)).Unit, 0, false, true);
                        }
                    }
                }
            }
        }

        protected override void OnProcessCreation()
        {
            base.OnProcessCreation();

            foreach (var pt in PlacedPoint.Value.Adjacents())
            {
                if (pt.HasValue)
                    Owner.TryAddTerritory(pt.Value);
            }
        }

        protected override double CalculateDamage(double originalDamage, Actor opposite, bool isMelee, bool isSkillAttack)
        {
            AttackTo(15, opposite, opposite.DefencePower, false, true);
            return originalDamage;
        }
    }

    public class HwanEmpireCityProductionFactory : ITileObjectProductionFactory
    {
        public static HwanEmpireCityProductionFactory Instance => _instance.Value;
        private static Lazy<HwanEmpireCityProductionFactory> _instance
            = new Lazy<HwanEmpireCityProductionFactory>(() => new HwanEmpireCityProductionFactory());

        private HwanEmpireCityProductionFactory()
        {
        }

        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner, 200, 20, 300, 50);
        }

        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.TileBuilding == null
                && point.Unit is Pioneer pioneer
                && pioneer.Owner == production.Owner;
        }

        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            // remove pioneer
            point.Unit.Destroy();

            return new HwanEmpireCity(owner, point);
        }
    }
}
