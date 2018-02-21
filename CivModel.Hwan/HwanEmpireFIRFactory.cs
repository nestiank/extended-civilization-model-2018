using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public sealed class HwanEmpireFIRFactory : InteriorBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("5AAB5AC2-59F4-4066-889F-3BFD419C25B4");
        public override Guid Guid => ClassGuid;

        public override double ProvidedLabor => 4;

        public HwanEmpireFIRFactory(CityBase city) : base(city) { }
    }

    public class HwanEmpireFIRFactoryProductionFactory : IInteriorBuildingProductionFactory
    {
        public static HwanEmpireFIRFactoryProductionFactory Instance => _instance.Value;
        private static Lazy<HwanEmpireFIRFactoryProductionFactory> _instance
            = new Lazy<HwanEmpireFIRFactoryProductionFactory>(() => new HwanEmpireFIRFactoryProductionFactory());
        private HwanEmpireFIRFactoryProductionFactory()
        {
        }
        public Production Create(Player owner)
        {
            return new InteriorBuildingProduction(this, owner, 10, 10, 10, 10);
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
