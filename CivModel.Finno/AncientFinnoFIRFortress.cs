using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public sealed class AncientFinnoFIRFortress : TileBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("F5AC55CF-C095-4525-9B87-111ED58856A2");
        public override Guid Guid => ClassGuid;

        public override double MaxHP => 30;

        public AncientFinnoFIRFortress(Player owner, Terrain.Point point) : base(owner, point) { }

        public override void PostTurn()
        {
            this.RemainHP = Math.Min(30, (this.RemainHP + 10));
        }
    }

    public class AncientFinnoFIRFortressProductionFactory : ITileObjectProductionFactory
    {
        public static AncientFinnoFIRFortressProductionFactory Instance => _instance.Value;
        private static Lazy<AncientFinnoFIRFortressProductionFactory> _instance
            = new Lazy<AncientFinnoFIRFortressProductionFactory>(() => new AncientFinnoFIRFortressProductionFactory());
        private AncientFinnoFIRFortressProductionFactory()
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
            return new AncientFinnoFIRFortress(owner, point);
        }
    }
}
