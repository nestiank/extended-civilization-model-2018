using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public sealed class AncientFinnoVigilant : InteriorBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("7A9A9079-361D-4ED6-92C1-24A0546AC029");
        public override Guid Guid => ClassGuid;

        public AncientFinnoVigilant(CityBase city) : base(city) { }
    }

    public class AncientFinnoVigilantProductionFactory : IInteriorBuildingProductionFactory
    {
        public static AncientFinnoVigilantProductionFactory Instance => _instance.Value;
        private static Lazy<AncientFinnoVigilantProductionFactory> _instance
            = new Lazy<AncientFinnoVigilantProductionFactory>(() => new AncientFinnoVigilantProductionFactory());
        private AncientFinnoVigilantProductionFactory()
        {
        }
        public Production Create(Player owner)
        {
            return new InteriorBuildingProduction(this, owner, 120, 20, 100, 25);
        }
        public bool IsPlacable(InteriorBuildingProduction production, CityBase city)
        {
            return true;
        }
        public InteriorBuilding CreateInteriorBuilding(CityBase city)
        {
            return new AncientFinnoVigilant(city);
        }
    }
}
