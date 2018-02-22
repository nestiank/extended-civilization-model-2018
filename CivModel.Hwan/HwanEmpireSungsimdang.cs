using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public sealed class HwanEmpireSungsimdang : InteriorBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("D6AE6381-5B65-47EB-B99B-B3BE2C232772");
        public override Guid Guid => ClassGuid;

        public HwanEmpireSungsimdang(CityBase city) : base(city) { }
    }

    public class HwanEmpireSungsimdangProductionFactory : IInteriorBuildingProductionFactory
    {
        public static HwanEmpireSungsimdangProductionFactory Instance => _instance.Value;
        private static Lazy<HwanEmpireSungsimdangProductionFactory> _instance
            = new Lazy<HwanEmpireSungsimdangProductionFactory>(() => new HwanEmpireSungsimdangProductionFactory());
        private HwanEmpireSungsimdangProductionFactory()
        {
        }
        public Production Create(Player owner)
        {
            return new InteriorBuildingProduction(this, owner, 30, 5, 50, 10);
        }
        public bool IsPlacable(InteriorBuildingProduction production, CityBase city)
        {
            return true;
        }
        public InteriorBuilding CreateInteriorBuilding(CityBase city)
        {
            return new HwanEmpireSungsimdang(city);
        }
    }
}
