using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public sealed class HwanEmpireKimchiFactory : TileBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("E491B144-C222-42ED-8617-C59A32E902AD");
        public override Guid Guid => ClassGuid;

        public override double MaxHP => 15;

        public HwanEmpireKimchiFactory(Player owner, Terrain.Point point) : base(owner, point) { }

        public override void PostTurn()
        {
            this.RemainHP = Math.Min(15, (this.RemainHP + 2));
        }
    }

    public class HwanEmpireKimchiFactoryProductionFactory : ITileObjectProductionFactory
    {
        public static HwanEmpireKimchiFactoryProductionFactory Instance => _instance.Value;
        private static Lazy<HwanEmpireKimchiFactoryProductionFactory> _instance
            = new Lazy<HwanEmpireKimchiFactoryProductionFactory>(() => new HwanEmpireKimchiFactoryProductionFactory());
        private HwanEmpireKimchiFactoryProductionFactory()
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
            return new HwanEmpireKimchiFactory(owner, point);
        }
    }
}
