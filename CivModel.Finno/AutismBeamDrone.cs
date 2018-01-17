using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class AutismBeamDrone : Unit
    {
        public override int MaxAP => 4;

        public AutismBeamDrone(Player owner) : base(owner)
        {
        }
    }

    public class AutismBeamDroneFactory : ITileObjectProductionFactory
    {
        private static Lazy<AutismBeamDroneFactory> _instance
            = new Lazy<AutismBeamDroneFactory>(() => new AutismBeamDroneFactory());
        public static AutismBeamDroneFactory Instance => _instance.Value;
        private AutismBeamDroneFactory()
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
            return new AutismBeamDrone(owner);
        }
    }
}
