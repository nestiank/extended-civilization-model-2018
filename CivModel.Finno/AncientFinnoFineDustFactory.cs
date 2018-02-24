using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public sealed class AncientFinnoFineDustFactory : TileBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("26F24220-2B77-4E81-A985-77F3BBC77832");
        public override Guid Guid => ClassGuid;

        public override double MaxHP => 20;

        public AncientFinnoFineDustFactory(Player owner, Terrain.Point point) : base(owner, point) { }
    }

    public class AncientFinnoFineDustFactoryProductionFactory : ITileObjectProductionFactory
    {
        public static AncientFinnoFineDustFactoryProductionFactory Instance => _instance.Value;
        private static Lazy<AncientFinnoFineDustFactoryProductionFactory> _instance
            = new Lazy<AncientFinnoFineDustFactoryProductionFactory>(() => new AncientFinnoFineDustFactoryProductionFactory());
        private AncientFinnoFineDustFactoryProductionFactory()
        {
        }
        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner, 20, 10, 20, 10);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.Unit == null
                 && point.TileBuilding is CityBase
                 && point.TileBuilding.Owner == production.Owner;
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new AncientFinnoFineDustFactory(owner, point);
        }
    }
}
