using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class CityCenterProductionFactory : ITileObjectProductionFactory
    {
        public static CityCenterProductionFactory Instance => _instance.Value;
        private static Lazy<CityCenterProductionFactory> _instance
            = new Lazy<CityCenterProductionFactory>(() => new CityCenterProductionFactory());

        private CityCenterProductionFactory()
        {
        }

        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner, 5, 2, 5, 2);
        }

        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.TileBuilding == null
                && point.Unit is Pioneer pioneer
                && pioneer.Owner == production.Owner;
        }

        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new CityCenter(owner, point);
        }
    }
}
