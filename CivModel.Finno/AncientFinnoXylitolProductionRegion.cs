using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public sealed class AncientFinnoXylitolProductionRegion : InteriorBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("335DA15B-C38D-4FF0-AE90-543369B3C525");
        public override Guid Guid => ClassGuid;

        public static InteriorBuildingConstants Constants = new InteriorBuildingConstants
        {
            GoldLogistics = 50
        };

        public AncientFinnoXylitolProductionRegion(CityBase city) : base(city, Constants) { }
    }

    public class AncientFinnoXylitolProductionRegionProductionFactory : IInteriorBuildingProductionFactory
    {
        public static AncientFinnoXylitolProductionRegionProductionFactory Instance => _instance.Value;
        private static Lazy<AncientFinnoXylitolProductionRegionProductionFactory> _instance
            = new Lazy<AncientFinnoXylitolProductionRegionProductionFactory>(() => new AncientFinnoXylitolProductionRegionProductionFactory());
        private AncientFinnoXylitolProductionRegionProductionFactory()
        {
        }

        public InteriorBuildingConstants Constants => AncientFinnoXylitolProductionRegion.Constants;

        public double TotalLaborCost => 100;
        public double LaborCapacityPerTurn => 20;
        public double TotalGoldCost => 70;
        public double GoldCapacityPerTurn => 15;

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
