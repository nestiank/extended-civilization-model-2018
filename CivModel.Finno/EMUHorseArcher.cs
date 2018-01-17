using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class EMUHorseArcher : Unit
    {
        public override int MaxAP => 4;

        public EMUHorseArcher(Player owner) : base(owner)
        {
        }
    }

    public class EMUHorseArcherProductionFactory : ITileObjectProductionFactory
    {
        private static Lazy<EMUHorseArcherProductionFactory> _instance
            = new Lazy<EMUHorseArcherProductionFactory>(() => new EMUHorseArcherProductionFactory());
        public static EMUHorseArcherProductionFactory Instance => _instance.Value;
        private EMUHorseArcherProductionFactory()
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
            return new EMUHorseArcher(owner);
        }
    }
}
