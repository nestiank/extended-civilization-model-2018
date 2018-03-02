using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Zap
{
    public sealed class Casino : TileBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("C60920C0-5F6E-4439-8A0E-419950B2DD1A");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxHP = 30,
            GoldLogistics = 20,
            LaborLogistics = 10,
            MaxHealPerTurn = 5
        };

        public Casino(Player owner, Terrain.Point point) : base(owner, Constants, point) { }

        public override void PostTurn()
        {
            base.PostTurn();
            Owner.Happiness += 1;
        }
    }

    public class CasinoProductionFactory : ITileObjectProductionFactory
    {
        public static CasinoProductionFactory Instance => _instance.Value;
        private static Lazy<CasinoProductionFactory> _instance
            = new Lazy<CasinoProductionFactory>(() => new CasinoProductionFactory());
        private CasinoProductionFactory()
        {
        }

        public ActorConstants ActorConstants => Casino.Constants;

        public double TotalLaborCost => 30;
        public double LaborCapacityPerTurn => 10;
        public double TotalGoldCost => 30;
        public double GoldCapacityPerTurn => 10;

        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.TileBuilding == null
                 && !IsCityNeer(production, point)
                 && point.TileOwner == production.Owner;
        }

        private bool IsCityNeer(TileObjectProduction production, Terrain.Point point)
        {
            int A = point.Position.A;
            int B = point.Position.B;
            int C = point.Position.C;


            if (RealAction(A + 1, B - 1, C, production, point))
                return true;
            else if (RealAction(A + 1, B, C - 1, production, point))
                return true;
            else if (RealAction(A, B + 1, C - 1, production, point))
                return true;
            else if (RealAction(A - 1, B + 1, C, production, point))
                return true;
            else if (RealAction(A - 1, B, C + 1, production, point))
                return true;
            else if (RealAction(A, B - 1, C + 1, production, point))
                return true;

            else if (RealAction(A + 2, B - 2, C, production, point))
                return true;
            else if (RealAction(A + 2, B - 1, C - 1, production, point))
                return true;
            else if (RealAction(A + 2, B, C - 2, production, point))
                return true;
            else if (RealAction(A + 1, B + 1, C - 2, production, point))
                return true;
            else if (RealAction(A, B + 2, C - 2, production, point))
                return true;
            else if (RealAction(A - 1, B + 2, C - 1, production, point))
                return true;
            else if (RealAction(A - 2, B + 2, C, production, point))
                return true;
            else if (RealAction(A - 2, B + 1, C + 1, production, point))
                return true;
            else if (RealAction(A - 2, B, C + 2, production, point))
                return true;
            else if (RealAction(A - 1, B - 1, C + 2, production, point))
                return true;
            else if (RealAction(A, B - 2, C + 2, production, point))
                return true;
            else if (RealAction(A + 1, B - 2, C + 1, production, point))
                return true;

            return false;
        }

        private bool RealAction(int A, int B, int C, TileObjectProduction production, Terrain.Point point)
        {
            if (0 <= B + (C + Math.Sign(C)) / 2 && B + (C + Math.Sign(C)) / 2 < point.Terrain.Width && 0 <= C && C < point.Terrain.Height)
            {
                if ((point.Terrain.GetPoint(A, B, C)).TileBuilding is CityBase)
                {
                    return true;
                }
            }
            return false;
        }

        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new Casino(owner, point);
        }
    }
}
