using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class LaboratoryBuilding : InteriorBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("39C928FE-721D-4BB9-B7F4-995F631923AF");
        public override Guid Guid => ClassGuid;

        public static readonly InteriorBuildingConstants Constants = new InteriorBuildingConstants {
            ResearchIncome = 10,
            ResearchCapacity = 100,
        };

        public LaboratoryBuilding(CityBase city) : base(city, Constants)
        {
        }
    }

    public class LaboratoryBuildingProductionFactory : IInteriorBuildingProductionFactory
    {
        public static LaboratoryBuildingProductionFactory Instance => _instance.Value;
        private static Lazy<LaboratoryBuildingProductionFactory> _instance
            = new Lazy<LaboratoryBuildingProductionFactory>(() => new LaboratoryBuildingProductionFactory());
        private LaboratoryBuildingProductionFactory()
        {
        }

        public Type ResultType => typeof(LaboratoryBuilding);
        public InteriorBuildingConstants Constants => LaboratoryBuilding.Constants;

        public double TotalLaborCost => 5;
        public double LaborCapacityPerTurn => 2;
        public double TotalGoldCost => 5;
        public double GoldCapacityPerTurn => 2;

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
            return new LaboratoryBuilding(city);
        }
    }
}
