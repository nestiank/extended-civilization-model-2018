using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.TileBuildings
{
    public class CityCenter : TileBuilding
    {
        private readonly Player _owner;
        public override Player Owner => _owner;

        public CityCenter(Player owner)
        {
            _owner = owner;
            _owner.Cities.Add(this);
        }
    }
}
