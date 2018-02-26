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

        public static InteriorBuildingConstants Constants = new InteriorBuildingConstants
        {
            ResearchCapacity = 100,
            ResearchIncome = 20,
            GoldLogistics = 50
        };


        public HwanEmpireCityCentralLab(CityBase city) : base(city, Constants) { }
    }

    public class HwanEmpireCityCentralLabProductionFactory : IInteriorBuildingProductionFactory
    {
        public static HwanEmpireCityCentralLabProductionFactory Instance => _instance.Value;
        private static Lazy<HwanEmpireCityCentralLabProductionFactory> _instance
            = new Lazy<HwanEmpireCityCentralLabProductionFactory>(() => new HwanEmpireCityCentralLabProductionFactory());
        private HwanEmpireCityCentralLabProductionFactory()
        {
        }

        public InteriorBuildingConstants Constants => HwanEmpireCityCentralLab.Constants;

        public double TotalLaborCost => 50;
        public double LaborCapacityPerTurn => 10;
        public double TotalGoldCost => 100;
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
            return new HwanEmpireCityCentralLab(city);
        }
    }
}
