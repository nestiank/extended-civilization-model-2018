using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public sealed class AncientFinnoFIRFactory : InteriorBuilding
    {
        public AncientFinnoFIRFactory(CityBase city) : base(city, typeof(AncientFinnoFIRFactory)) { }
    }

    public class AncientFinnoFIRFactoryProductionFactory : IInteriorBuildingProductionFactory
    {
        public static AncientFinnoFIRFactoryProductionFactory Instance => _instance.Value;
        private static Lazy<AncientFinnoFIRFactoryProductionFactory> _instance
            = new Lazy<AncientFinnoFIRFactoryProductionFactory>(() => new AncientFinnoFIRFactoryProductionFactory());
        private AncientFinnoFIRFactoryProductionFactory()
        {
        }

        public Type ResultType => typeof(AncientFinnoFIRFactory);

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
