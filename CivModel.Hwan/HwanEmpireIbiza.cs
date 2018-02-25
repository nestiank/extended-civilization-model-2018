using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public sealed class HwanEmpireIbiza : TileBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("9B1A6066-6DA6-438F-A285-30D26EBD7828");
        public override Guid Guid => ClassGuid;

        public override double MaxHP => 30;

        public HwanEmpireIbiza(Player owner, Terrain.Point point) : base(owner, point) { }

        public override void PostTurn()
        {
            this.RemainHP = Math.Min(30, (this.RemainHP + 5));
        }   
    }

    public class HwanEmpireIbizaProductionFactory : ITileObjectProductionFactory
    {
        public static HwanEmpireIbizaProductionFactory Instance => _instance.Value;
        private static Lazy<HwanEmpireIbizaProductionFactory> _instance
            = new Lazy<HwanEmpireIbizaProductionFactory>(() => new HwanEmpireIbizaProductionFactory());
        private HwanEmpireIbizaProductionFactory()
        {
        }
        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner, 30, 10, 30, 10);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.Unit == null
                 && point.TileBuilding is CityBase
                 && point.TileBuilding.Owner == production.Owner;
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new HwanEmpireIbiza(owner, point);
        }
    }
}
