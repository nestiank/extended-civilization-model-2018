using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public sealed class HwanEmpireVigilant : InteriorBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("F0D6189E-EE05-4BD6-B9A8-A7C5E1AE374E");
        public override Guid Guid => ClassGuid;

        public static InteriorBuildingConstants Constants = new InteriorBuildingConstants
        {
            GoldLogistics = 50
        };

        public HwanEmpireVigilant(CityBase city) : base(city, Constants) { }
    }

    public class HwanEmpireVigilantProductionFactory : IInteriorBuildingProductionFactory
    {
        public static HwanEmpireVigilantProductionFactory Instance => _instance.Value;
        private static Lazy<HwanEmpireVigilantProductionFactory> _instance
            = new Lazy<HwanEmpireVigilantProductionFactory>(() => new HwanEmpireVigilantProductionFactory());
        private HwanEmpireVigilantProductionFactory()
        {
        }

        public InteriorBuildingConstants Constants => HwanEmpireVigilant.Constants;

        public double TotalLaborCost => 60;
        public double LaborCapacityPerTurn => 20;
        public double TotalGoldCost => 60;
        public double GoldCapacityPerTurn => 20;


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
            return new HwanEmpireVigilant(city);
        }
    }
}
