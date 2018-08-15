using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public sealed class AncientFinnoXylitolProductionRegion : InteriorBuilding
    {
        public AncientFinnoXylitolProductionRegion(CityBase city) : base(city, typeof(AncientFinnoXylitolProductionRegion)) { }
    }

    public class AncientFinnoXylitolProductionRegionProductionFactory : IInteriorBuildingProductionFactory
    {
        public static AncientFinnoXylitolProductionRegionProductionFactory Instance => _instance.Value;
        private static Lazy<AncientFinnoXylitolProductionRegionProductionFactory> _instance
            = new Lazy<AncientFinnoXylitolProductionRegionProductionFactory>(() => new AncientFinnoXylitolProductionRegionProductionFactory());
        private AncientFinnoXylitolProductionRegionProductionFactory()
        {
        }

        public Type ResultType => typeof(AncientFinnoXylitolProductionRegion);

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
            return new AncientFinnoXylitolProductionRegion(city);
        }
    }
}
