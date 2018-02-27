using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public sealed class AncientFinnoFIRFactory : InteriorBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("C07FCB2A-4D41-45F2-AF03-8663E230F3A6");
        public override Guid Guid => ClassGuid;

        public static InteriorBuildingConstants Constants = new InteriorBuildingConstants
        {
            ProvidedLabor = 5,
            GoldLogistics = 20
        };


        public AncientFinnoFIRFactory(CityBase city) : base(city, Constants) { }
    }

    public class AncientFinnoFIRFactoryProductionFactory : IInteriorBuildingProductionFactory
    {
        public static AncientFinnoFIRFactoryProductionFactory Instance => _instance.Value;
        private static Lazy<AncientFinnoFIRFactoryProductionFactory> _instance
            = new Lazy<AncientFinnoFIRFactoryProductionFactory>(() => new AncientFinnoFIRFactoryProductionFactory());
        private AncientFinnoFIRFactoryProductionFactory()
        {
        }

        public InteriorBuildingConstants Constants => AncientFinnoFIRFactory.Constants;

        public double TotalLaborCost => 10;
        public double LaborCapacityPerTurn => 10;
        public double TotalGoldCost => 10;
        public double GoldCapacityPerTurn => 10;

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
            return new AncientFinnoFIRFactory(city);
        }
    }
}
