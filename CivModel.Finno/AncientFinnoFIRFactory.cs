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

        public override double ProvidedLabor => 5;

        public AncientFinnoFIRFactory(CityBase city) : base(city) { }
    }

    public class AncientFinnoFIRFactoryProductionFactory : IInteriorBuildingProductionFactory
    {
        public static AncientFinnoFIRFactoryProductionFactory Instance => _instance.Value;
        private static Lazy<AncientFinnoFIRFactoryProductionFactory> _instance
            = new Lazy<AncientFinnoFIRFactoryProductionFactory>(() => new AncientFinnoFIRFactoryProductionFactory());
        private AncientFinnoFIRFactoryProductionFactory()
        {
        }
        public Production Create(Player owner)
        {
            return new InteriorBuildingProduction(this, owner, 10, 10, 10, 10);
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
