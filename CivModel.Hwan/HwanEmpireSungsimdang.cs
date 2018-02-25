using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public sealed class HwanEmpireSungsimdang : InteriorBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("D6AE6381-5B65-47EB-B99B-B3BE2C232772");
        public override Guid Guid => ClassGuid;

        public HwanEmpireSungsimdang(CityBase city) : base(city) { }

        public int SoBoRo = 0;

        public override void PostTurn()
        {
            Random r = new Random();

            int GetSoBoRo = r.Next(1, 100);

            if (GetSoBoRo <= 30)
                this.SoBoRo = this.SoBoRo + 1;

            if(this.SoBoRo >= 3)
            {
                this.SoBoRo = 0;
                if (GetSoBoRo % 2 == 0)
                {
                    
                }

                else if(GetSoBoRo % 2 == 1)
                {
                    City.Population = City.Population + 1;
                }
            }
        }
    }

    public class HwanEmpireSungsimdangProductionFactory : IInteriorBuildingProductionFactory
    {
        public static HwanEmpireSungsimdangProductionFactory Instance => _instance.Value;
        private static Lazy<HwanEmpireSungsimdangProductionFactory> _instance
            = new Lazy<HwanEmpireSungsimdangProductionFactory>(() => new HwanEmpireSungsimdangProductionFactory());
        private HwanEmpireSungsimdangProductionFactory()
        {
        }
        public Production Create(Player owner)
        {
            return new InteriorBuildingProduction(this, owner, 30, 5, 50, 10);
        }
        public bool IsPlacable(InteriorBuildingProduction production, CityBase city)
        {
            return true;
        }
        public InteriorBuilding CreateInteriorBuilding(CityBase city)
        {
            return new HwanEmpireSungsimdang(city);
        }
    }
}
