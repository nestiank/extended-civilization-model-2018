using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public sealed class HwanEmpireVigilant : InteriorBuilding
    {
        public HwanEmpireVigilant(CityBase city)
            : base(city, typeof(HwanEmpireVigilant))
        {
            city.MaxHP += 50;
            city.RemainHP += 50;
        }

        protected override void OnBeforeDestroy()
        {
            City.MaxHP -= 50;
            base.OnBeforeDestroy();
        }
    }

    public class HwanEmpireVigilantProductionFactory : IInteriorBuildingProductionFactory
    {
        public static HwanEmpireVigilantProductionFactory Instance => _instance.Value;
        private static Lazy<HwanEmpireVigilantProductionFactory> _instance
            = new Lazy<HwanEmpireVigilantProductionFactory>(() => new HwanEmpireVigilantProductionFactory());
        private HwanEmpireVigilantProductionFactory()
        {
        }

        public Type ResultType => typeof(HwanEmpireVigilant);

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
