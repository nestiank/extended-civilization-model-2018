using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Zap
{
    public sealed class CityLab : InteriorBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("7D49A0B3-976E-454A-941A-E8B5FFBEAC02");
        public override Guid Guid => ClassGuid;

        public static InteriorBuildingConstants Constants = new InteriorBuildingConstants
        {
            ResearchCapacity = 100,
            ResearchIncome = 20,
            GoldLogistics = 50
        };


        public CityLab(CityBase city) : base(city, Constants) { }
    }

    public class CityLabProductionFactory : IInteriorBuildingProductionFactory
    {
        public static CityLabProductionFactory Instance => _instance.Value;
        private static Lazy<CityLabProductionFactory> _instance
            = new Lazy<CityLabProductionFactory>(() => new CityLabProductionFactory());
        private CityLabProductionFactory()
        {
        }

        public InteriorBuildingConstants Constants => CityLab.Constants;

        public double TotalLaborCost => 100;
        public double LaborCapacityPerTurn => 20;
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
            return new CityLab(city);
        }
    }
}
