using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class CityCenter : TileBuilding
    {
        private readonly Player _owner;
        public override Player Owner => _owner;

        private List<IProductionFactory> _availableProduction = new List<IProductionFactory>();
        public IReadOnlyList<IProductionFactory> AvailableProduction => _availableProduction;

        public CityCenter(Player owner)
        {
            _owner = owner;
            _owner.Cities.Add(this);

            _availableProduction.Add(PioneerProductionFactory.Instance);
        }
    }
}
