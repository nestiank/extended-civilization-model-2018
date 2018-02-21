using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public sealed class HwanEmpireCityCentralLab : InteriorBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("BE574FAE-BB83-4D8C-9547-BDA426F24C4A");
        public override Guid Guid => ClassGuid;

        public override double ProvidedResearchIncome => 20;

        public HwanEmpireCityCentralLab(CityBase city) : base(city) { }
    }

    public class HwanEmpireCityCentralLabProductionFactory : IInteriorBuildingProductionFactory
    {
        public static HwanEmpireCityCentralLabProductionFactory Instance => _instance.Value;
        private static Lazy<HwanEmpireCityCentralLabProductionFactory> _instance
            = new Lazy<HwanEmpireCityCentralLabProductionFactory>(() => new HwanEmpireCityCentralLabProductionFactory());
        private HwanEmpireCityCentralLabProductionFactory()
        {
        }
        public Production Create(Player owner)
        {
            return new InteriorBuildingProduction(this, owner, 50, 10, 100, 20);
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
