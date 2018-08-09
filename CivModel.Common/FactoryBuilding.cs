using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class FactoryBuilding : InteriorBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("A2AE33B4-5543-4751-8681-E958DFC1A511");
        public override Guid Guid => ClassGuid;

        public static InteriorBuildingConstants Constants = new InteriorBuildingConstants {
            ProvidedLabor = 70
        };

        public FactoryBuilding(CityBase city) : base(city, Constants)
        {
        }
    }

    public class FactoryBuildingProductionFactory : IInteriorBuildingProductionFactory
    {
        public static FactoryBuildingProductionFactory Instance => _instance.Value;
        private static Lazy<FactoryBuildingProductionFactory> _instance
            = new Lazy<FactoryBuildingProductionFactory>(() => new FactoryBuildingProductionFactory());
        private FactoryBuildingProductionFactory()
        {
        }

        public Type ResultType => typeof(FactoryBuilding);
        public InteriorBuildingConstants Constants => FactoryBuilding.Constants;

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
            return new FactoryBuilding(city);
        }
    }
}
