using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public sealed class AncientFinnoLabortory : InteriorBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("2E5AF09B-7E81-451F-ADBE-11A44EDA49A6");
        public override Guid Guid => ClassGuid;


        public static InteriorBuildingConstants Constants = new InteriorBuildingConstants
        {
            ResearchCapacity = 200,
            ResearchIncome = 20,
            GoldLogistics = 50
        };

        public AncientFinnoLabortory(CityBase city) : base(city, Constants) { }
    }

    public class AncientFinnoLabortoryProductionFactory : IInteriorBuildingProductionFactory
    {
        public static AncientFinnoLabortoryProductionFactory Instance => _instance.Value;
        private static Lazy<AncientFinnoLabortoryProductionFactory> _instance
            = new Lazy<AncientFinnoLabortoryProductionFactory>(() => new AncientFinnoLabortoryProductionFactory());
        private AncientFinnoLabortoryProductionFactory()
        {
        }


        public InteriorBuildingConstants Constants => AncientFinnoLabortory.Constants;

        public double TotalLaborCost => 120;
        public double LaborCapacityPerTurn => 20;
        public double TotalGoldCost => 100;
        public double GoldCapacityPerTurn => 25;


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
            return new AncientFinnoLabortory(city);
        }
    }
}
