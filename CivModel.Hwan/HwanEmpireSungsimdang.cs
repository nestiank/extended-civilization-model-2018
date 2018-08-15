using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public sealed class HwanEmpireSungsimdang : InteriorBuilding
    {
        public HwanEmpireSungsimdang(CityBase city) : base(city, typeof(HwanEmpireSungsimdang)) { }

        public int SoBoRo = 0;

        protected override void FixedPostTurn()
        {
            base.FixedPostTurn();

            int GetSoBoRo = Game.Random.Next(100);

            if (GetSoBoRo <= 30)
                this.SoBoRo = this.SoBoRo + 1;

            if(this.SoBoRo >= 3)
            {
                this.SoBoRo = 0;
                if (GetSoBoRo % 2 == 0)
                {
                    Owner.Gold += 30;
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

        public Type ResultType => typeof(HwanEmpireSungsimdang);

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
            return new HwanEmpireSungsimdang(city);
        }
    }
}
