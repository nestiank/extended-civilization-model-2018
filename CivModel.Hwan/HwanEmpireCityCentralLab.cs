using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public sealed class HwanEmpireCityCentralLab : InteriorBuilding
    {
        public HwanEmpireCityCentralLab(CityBase city) : base(city, typeof(HwanEmpireCityCentralLab)) { }
    }

    public class HwanEmpireCityCentralLabProductionFactory : IInteriorBuildingProductionFactory
    {
        public static HwanEmpireCityCentralLabProductionFactory Instance => _instance.Value;
        private static Lazy<HwanEmpireCityCentralLabProductionFactory> _instance
            = new Lazy<HwanEmpireCityCentralLabProductionFactory>(() => new HwanEmpireCityCentralLabProductionFactory());
        private HwanEmpireCityCentralLabProductionFactory()
        {
        }

        public Type ResultType => typeof(HwanEmpireCityCentralLab);

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
            return new HwanEmpireCityCentralLab(city);
        }
    }
}
