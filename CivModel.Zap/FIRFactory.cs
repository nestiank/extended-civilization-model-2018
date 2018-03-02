using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Zap
{
    public sealed class FIRFactory : InteriorBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("3948E644-33B9-4FA8-B3BA-002EAD04DF0E");
        public override Guid Guid => ClassGuid;

        public static InteriorBuildingConstants Constants = new InteriorBuildingConstants
        {
            ProvidedLabor = 3,
            GoldLogistics = 20
        };

        public FIRFactory(CityBase city) : base(city, Constants) { }
    }

    public class FIRFactoryProductionFactory : IInteriorBuildingProductionFactory
    {
        public static FIRFactoryProductionFactory Instance => _instance.Value;
        private static Lazy<FIRFactoryProductionFactory> _instance
            = new Lazy<FIRFactoryProductionFactory>(() => new FIRFactoryProductionFactory());
        private FIRFactoryProductionFactory()
        {
        }

        public Type ResultType => typeof(FIRFactory);
        public InteriorBuildingConstants Constants => FIRFactory.Constants;

        public double TotalLaborCost => 10;
        public double LaborCapacityPerTurn => 10;
        public double TotalGoldCost => 10;
        public double GoldCapacityPerTurn => 10;

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
            return new FIRFactory(city);
        }
    }
}
