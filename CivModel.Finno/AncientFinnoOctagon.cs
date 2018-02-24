using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public sealed class AncientFinnoOctagon : TileBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("AD9DD607-2972-40F7-8596-391955621CB3");
        public override Guid Guid => ClassGuid;

        public override double MaxHP => 20;

        public AncientFinnoOctagon(Player owner, Terrain.Point point) : base(owner, point) { }
    }

    public class AncientFinnoOctagonProductionFactory : ITileObjectProductionFactory
    {
        public static AncientFinnoOctagonProductionFactory Instance => _instance.Value;
        private static Lazy<AncientFinnoOctagonProductionFactory> _instance
            = new Lazy<AncientFinnoOctagonProductionFactory>(() => new AncientFinnoOctagonProductionFactory());
        private AncientFinnoOctagonProductionFactory()
        {
        }
        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner, 20, 15, 60, 20);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.Unit == null
                 && point.TileBuilding is CityBase
                 && point.TileBuilding.Owner == production.Owner;
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new AncientFinnoOctagon(owner, point);
        }
    }
}
