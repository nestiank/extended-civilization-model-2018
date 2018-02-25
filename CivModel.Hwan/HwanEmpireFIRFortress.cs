using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public sealed class HwanEmpireFIRFortress : TileBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("B6BBF4C9-26F4-48C7-87EE-15E6F21B2DC2");
        public override Guid Guid => ClassGuid;

        public override double MaxHP => 30;

        public HwanEmpireFIRFortress(Player owner, Terrain.Point point) : base(owner, point) { }

        public override void PostTurn()
        {
            this.RemainHP = Math.Min(30, (this.RemainHP + 10));
        }
    }

    public class HwanEmpireFIRFortressProductionFactory : ITileObjectProductionFactory
    {
        public static HwanEmpireFIRFortressProductionFactory Instance => _instance.Value;
        private static Lazy<HwanEmpireFIRFortressProductionFactory> _instance
            = new Lazy<HwanEmpireFIRFortressProductionFactory>(() => new HwanEmpireFIRFortressProductionFactory());
        private HwanEmpireFIRFortressProductionFactory()
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
            return new HwanEmpireFIRFortress(owner, point);
        }
    }
}
