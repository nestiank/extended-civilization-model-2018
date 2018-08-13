using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Zap
{
    public sealed class CityLab : InteriorBuilding
    {
        public CityLab(CityBase city) : base(city, typeof(CityLab)) { }
    }

    public class CityLabProductionFactory : IInteriorBuildingProductionFactory
    {
        public static CityLabProductionFactory Instance => _instance.Value;
        private static Lazy<CityLabProductionFactory> _instance
            = new Lazy<CityLabProductionFactory>(() => new CityLabProductionFactory());
        private CityLabProductionFactory()
        {
        }

        public Type ResultType => typeof(CityLab);

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
            return new CityLab(city);
        }
    }
}
