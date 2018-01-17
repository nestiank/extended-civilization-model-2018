using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class DecentralizedMilitary : Unit
    {
        public override int MaxAP => 4;

        public DecentralizedMilitary(Player owner) : base(owner)
        {
        }
    }

    public class DecentralizedMilitaryProductionFactory : ITileObjectProductionFactory
    {
        private static Lazy<DecentralizedMilitaryProductionFactory> _instance
            = new Lazy<DecentralizedMilitaryProductionFactory>(() => new DecentralizedMilitaryProductionFactory());
        public static DecentralizedMilitaryProductionFactory Instance => _instance.Value;
        private DecentralizedMilitaryProductionFactory()
        {
        }
        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner, 7, 3);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.Unit == null
                && point.TileBuilding is CityCenter
                && point.TileBuilding.Owner == production.Owner;
        }
        public TileObject CreateTileObject(Player owner)
        {
            return new DecentralizedMilitary(owner);
        }
    }
}
