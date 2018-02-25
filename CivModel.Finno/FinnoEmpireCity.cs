using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public sealed class FinnoEmpireCity : CityBase
    {
        public static Guid ClassGuid { get; } = new Guid("300E06FD-B656-46DC-A668-BB36C75E3086");
        public override Guid Guid => ClassGuid;

        public override double MaxHP => 500;

        public override void PostTurn()
        {
            this.RemainHP = Math.Min(500, (this.RemainHP + 50));
        }

        public override IReadOnlyList<IActorAction> SpecialActs => _specialActs;
        private readonly IActorAction[] _specialActs = new IActorAction[1];

        public FinnoEmpireCity(Player player, Terrain.Point point) : base(player, point)
        {
            _specialActs[0] = new FinnoEmpireCityAction(this);
        }

        private class FinnoEmpireCityAction : IActorAction
        {
            private readonly FinnoEmpireCity _owner;
            public Actor Owner => _owner;

            public bool IsParametered => false;

            public FinnoEmpireCityAction(FinnoEmpireCity owner)
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

            public void RealAction(int A, int B, int C)
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

   
    }

    public class FinnoEmpireCityProductionFactory : ITileObjectProductionFactory
    {
        public static FinnoEmpireCityProductionFactory Instance => _instance.Value;
        private static Lazy<FinnoEmpireCityProductionFactory> _instance
            = new Lazy<FinnoEmpireCityProductionFactory>(() => new FinnoEmpireCityProductionFactory());

        private FinnoEmpireCityProductionFactory()
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

            return new FinnoEmpireCity(owner, point);
        }
    }
}
