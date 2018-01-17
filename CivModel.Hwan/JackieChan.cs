using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class JackieChan : Unit
    {
        public override int MaxAP => 4;

        public JackieChan(Player owner) : base(owner)
        {
        }
    }

    public class JackieChanProductionFactory : ITileObjectProductionFactory
    {
        private static Lazy<JackieChanProductionFactory> _instance
            = new Lazy<JackieChanProductionFactory>(() => new JackieChanProductionFactory());
        public static JackieChanProductionFactory Instance => _instance.Value;
        private JackieChanProductionFactory()
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
            return new JackieChan(owner);
        }
    }
}