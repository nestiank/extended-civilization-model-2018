using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public sealed class HwanEmpireFIRFactory : InteriorBuilding
    {
        public HwanEmpireFIRFactory(CityBase city) : base(city, typeof(HwanEmpireFIRFactory)) { }
    }

    public class HwanEmpireFIRFactoryProductionFactory : IInteriorBuildingProductionFactory
    {
        public static HwanEmpireFIRFactoryProductionFactory Instance => _instance.Value;
        private static Lazy<HwanEmpireFIRFactoryProductionFactory> _instance
            = new Lazy<HwanEmpireFIRFactoryProductionFactory>(() => new HwanEmpireFIRFactoryProductionFactory());
        private HwanEmpireFIRFactoryProductionFactory()
        {
        }

        public Type ResultType => typeof(HwanEmpireFIRFactory);

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
            return new HwanEmpireFIRFactory(city);
        }
    }
}
