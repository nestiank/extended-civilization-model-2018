using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Zap
{
    public sealed class FIRFactory : InteriorBuilding
    {
        public FIRFactory(CityBase city) : base(city, typeof(FIRFactory)) { }
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
