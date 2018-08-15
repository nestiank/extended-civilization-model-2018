using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public sealed class HwanEmpireIbiza : TileBuilding
    {
        public HwanEmpireIbiza(Player owner, Terrain.Point point, Player donator = null)
            : base(owner, typeof(HwanEmpireIbiza), point, donator)
        {
        }
    }

    public class HwanEmpireIbizaProductionFactory : ITileBuildingProductionFactory
    {
        public static HwanEmpireIbizaProductionFactory Instance => _instance.Value;
        private static Lazy<HwanEmpireIbizaProductionFactory> _instance
            = new Lazy<HwanEmpireIbizaProductionFactory>(() => new HwanEmpireIbizaProductionFactory());
        private HwanEmpireIbizaProductionFactory()
        {
        }

        public Type ResultType => typeof(HwanEmpireIbiza);

        public Production Create(Player owner)
        {
            return new TileBuildingProduction(this, owner);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.TileBuilding == null && !IsCityNeer(production, point) && production.Owner.IsAlliedWith(point.TileOwner);
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new HwanEmpireIbiza(owner, point);
        }
        public TileBuilding CreateDonation(Player owner, Terrain.Point point, Player donator)
        {
            return new HwanEmpireIbiza(owner, point, donator);
        }

        private bool IsCityNeer(TileObjectProduction production, Terrain.Point point)
        {
            int A = point.Position.A;
            int B = point.Position.B;
            int C = point.Position.C;


            if (IsInDisOne(production, point))
                return true;

            else if (IsInDisTwo(production, point))
                return true;               

            return false;
        }

        private bool IsInDisOne(TileObjectProduction production, Terrain.Point point)
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

            return false;
        }

        private bool IsInDisTwo(TileObjectProduction production, Terrain.Point point)
        {
            int A = point.Position.A;
            int B = point.Position.B;
            int C = point.Position.C;

            if (RealAction(A + 2, B - 2, C, production, point))
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
    }
}
