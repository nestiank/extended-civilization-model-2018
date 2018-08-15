using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public sealed class AncientFinnoLabortory : InteriorBuilding
    {
        public AncientFinnoLabortory(CityBase city) : base(city, typeof(AncientFinnoLabortory)) { }
    }

    public class AncientFinnoLabortoryProductionFactory : IInteriorBuildingProductionFactory
    {
        public static AncientFinnoLabortoryProductionFactory Instance => _instance.Value;
        private static Lazy<AncientFinnoLabortoryProductionFactory> _instance
            = new Lazy<AncientFinnoLabortoryProductionFactory>(() => new AncientFinnoLabortoryProductionFactory());
        private AncientFinnoLabortoryProductionFactory()
        {
        }

        public Type ResultType => typeof(AncientFinnoLabortory);


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
            return new AncientFinnoLabortory(city);
        }
    }
}
