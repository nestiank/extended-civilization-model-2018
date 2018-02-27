using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public sealed class AncientFinnoVigilant : InteriorBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("7A9A9079-361D-4ED6-92C1-24A0546AC029");
        public override Guid Guid => ClassGuid;

        public static InteriorBuildingConstants Constants = new InteriorBuildingConstants
        {
            GoldLogistics = 50
        };

        public AncientFinnoVigilant(CityBase city) : base(city, Constants) { }

        public override void PostTurn()
        {
            base.PostTurn();
            Random r = new Random();

            int GetUnit = r.Next(1, 100);

            if (GetUnit <= 20)
            {

            }
        }

        private void SendUnit(int rand)
        {
            int A = this.City.PlacedPoint.Value.Position.A;
            int B = this.City.PlacedPoint.Value.Position.B;
            int C = this.City.PlacedPoint.Value.Position.C;

            bool IsItOk = false;

            int PointA = A;
            int PointB = B;
            int PointC = C;

            if (!CheckUnit(A + 1, B - 1, C))
            {
                IsItOk = true;

                PointA = A + 1;
                PointB = B - 1;
                PointC = C;
            }

            else if (!CheckUnit(A + 1, B, C - 1))
            {
                IsItOk = true;

                PointA = A + 1;
                PointB = B;
                PointC = C - 1;
            }

            else if (!CheckUnit(A, B + 1, C - 1))
            {
                IsItOk = true;

                PointA = A;
                PointB = B + 1;
                PointC = C - 1;
            }

            else if (!CheckUnit(A - 1, B + 1, C))
            {
                IsItOk = true;

                PointA = A - 1;
                PointB = B + 1;
                PointC = C;
            }

            else if (!CheckUnit(A - 1, B, C + 1))
            {
                IsItOk = true;

                PointA = A - 1;
                PointB = B;
                PointC = C + 1;
            }

            else if (!CheckUnit(A, B - 1, C + 1))
            {
                IsItOk = true;

                PointA = A;
                PointB = B - 1;
                PointC = C + 1;
            }

            if (IsItOk)
            {
                if (rand % 2 == 0)
                    new DecentralizedMilitary(Owner, this.City.PlacedPoint.Value.Terrain.GetPoint(PointA, PointB, PointC));


                else
                    new EMUHorseArcher(Owner, this.City.PlacedPoint.Value.Terrain.GetPoint(PointA, PointB, PointC));

            }
        }

        private bool CheckUnit(int A, int B, int C)
        {
            if (0 <= B + (C + Math.Sign(C)) / 2 && B + (C + Math.Sign(C)) / 2 < this.City.PlacedPoint.Value.Terrain.Width && 0 <= C && C < this.City.PlacedPoint.Value.Terrain.Height)
            {
                if ((this.City.PlacedPoint.Value.Terrain.GetPoint(A, B, C)).Unit != null)
                    return false;

                else
                    return true;
            }

            return false;
        }
    }

    public class AncientFinnoVigilantProductionFactory : IInteriorBuildingProductionFactory
    {
        public static AncientFinnoVigilantProductionFactory Instance => _instance.Value;
        private static Lazy<AncientFinnoVigilantProductionFactory> _instance
            = new Lazy<AncientFinnoVigilantProductionFactory>(() => new AncientFinnoVigilantProductionFactory());
        private AncientFinnoVigilantProductionFactory()
        {
        }

        public InteriorBuildingConstants Constants => AncientFinnoVigilant.Constants;

        public double TotalLaborCost => 100;
        public double LaborCapacityPerTurn => 20;
        public double TotalGoldCost => 100;
        public double GoldCapacityPerTurn => 20;

        public Production Create(Player owner)
        {
            return new InteriorBuildingProduction(this, owner);
        }
        public bool IsPlacable(InteriorBuildingProduction production, CityBase city)
        {
            return true;
        }
        public InteriorBuilding CreateInteriorBuilding(CityBase city)
        {
            return new AncientFinnoVigilant(city);
        }
    }
}
