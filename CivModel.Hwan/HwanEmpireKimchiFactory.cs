using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public sealed class HwanEmpireKimchiFactory : TileBuilding
    {
        public HwanEmpireKimchiFactory(Player owner, Terrain.Point point, Player donator = null)
            : base(owner, typeof(HwanEmpireKimchiFactory), point, donator)
        {
        }

        protected override void OnBeforePillaged(Actor pillager)
        {
            base.OnBeforePillaged(pillager);
            pillager.Die(Owner);
        }
    }

    public class HwanEmpireKimchiFactoryProductionFactory : ITileBuildingProductionFactory
    {
        public static HwanEmpireKimchiFactoryProductionFactory Instance => _instance.Value;
        private static Lazy<HwanEmpireKimchiFactoryProductionFactory> _instance
            = new Lazy<HwanEmpireKimchiFactoryProductionFactory>(() => new HwanEmpireKimchiFactoryProductionFactory());
        private HwanEmpireKimchiFactoryProductionFactory()
        {
        }

        public Type ResultType => typeof(HwanEmpireKimchiFactory);

        public Production Create(Player owner)
        {
            return new TileBuildingProduction(this, owner);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.TileBuilding == null && production.Owner.IsAlliedWith(point.TileOwner);
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new HwanEmpireKimchiFactory(owner, point);
        }
        public TileBuilding CreateDonation(Player owner, Terrain.Point point, Player donator)
        {
            return new HwanEmpireKimchiFactory(owner, point, donator);
        }
    }
}
