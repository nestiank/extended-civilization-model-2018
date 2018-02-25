using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public sealed class HwanEmpireCity : CityBase
    {
        public static Guid ClassGuid { get; } = new Guid("D0A84907-885A-44C2-8E4C-077744E1E0C3");
        public override Guid Guid => ClassGuid;

        public override double MaxHP => 500;

        public HwanEmpireCity(Player player, Terrain.Point point) : base(player, point)
        {
        }

        protected override void OnProcessCreation()
        {
            base.OnProcessCreation();

            foreach (var pt in PlacedPoint.Value.Adjacents())
            {
                if (pt.HasValue)
                    Owner.TryAddTerritory(pt.Value);
            }
        }
        public override void PostTurn()
        {
            this.RemainHP = Math.Min(500, (this.RemainHP + 20));
        }
    }

    public class HwanEmpireCityProductionFactory : ITileObjectProductionFactory
    {
        public static HwanEmpireCityProductionFactory Instance => _instance.Value;
        private static Lazy<HwanEmpireCityProductionFactory> _instance
            = new Lazy<HwanEmpireCityProductionFactory>(() => new HwanEmpireCityProductionFactory());

        private HwanEmpireCityProductionFactory()
        {
        }

        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner, 200, 20, 300, 50);
        }

        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.TileBuilding == null
                && point.Unit is Pioneer pioneer
                && pioneer.Owner == production.Owner;
        }

        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            // remove pioneer
            point.Unit.Destroy();

            return new HwanEmpireCity(owner, point);
        }
    }
}
