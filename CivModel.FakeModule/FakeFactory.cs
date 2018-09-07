using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.FakeModule
{
    public sealed class FakeFactory : InteriorBuilding
    {
        public FakeFactory(CityBase city)
            : base(city, typeof(FakeFactory))
        {
        }
    }

    public class FakeFactoryProductionFactory : IInteriorBuildingProductionFactory
    {
        public static FakeFactoryProductionFactory Instance => _instance.Value;
        private static Lazy<FakeFactoryProductionFactory> _instance
            = new Lazy<FakeFactoryProductionFactory>(() => new FakeFactoryProductionFactory());
        private FakeFactoryProductionFactory()
        {
        }

        public Type ResultType => typeof(FakeFactory);

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
            return new FakeFactory(city);
        }
    }
}
