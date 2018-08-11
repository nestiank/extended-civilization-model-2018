using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public sealed class FakeLaboratory : InteriorBuilding
    {
        public FakeLaboratory(CityBase city)
            : base(city, typeof(FakeLaboratory))
        {
        }
    }

    public class FakeLaboratoryProductionFactory : IInteriorBuildingProductionFactory
    {
        public static FakeLaboratoryProductionFactory Instance => _instance.Value;
        private static Lazy<FakeLaboratoryProductionFactory> _instance
            = new Lazy<FakeLaboratoryProductionFactory>(() => new FakeLaboratoryProductionFactory());
        private FakeLaboratoryProductionFactory()
        {
        }

        public Type ResultType => typeof(FakeLaboratory);

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
            return new FakeLaboratory(city);
        }
    }
}
