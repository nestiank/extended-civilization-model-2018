using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public sealed class CityCenter : CityBase
    {
        public CityCenter(Player player, Terrain.Point point)
            : base(player, typeof(CityCenter), point, null)
        {
        }

        public override void OnAfterProduce(Production production)
        {
            base.OnAfterProduce(production);

            new FakeFactory(this);

            foreach (var pt in PlacedPoint.Value.Adjacents())
            {
                if (pt.HasValue)
                    Owner.TryAddTerritory(pt.Value);
            }
        }

        protected override void OnAfterChangeOwner(Player prevOwner)
        {
            base.OnAfterChangeOwner(prevOwner);
            StealAdjacentTerritory(prevOwner);
        }
    }

    public class CityCenterProductionFactory : IActorProductionFactory
    {
        public static CityCenterProductionFactory Instance => _instance.Value;
        private static Lazy<CityCenterProductionFactory> _instance
            = new Lazy<CityCenterProductionFactory>(() => new CityCenterProductionFactory());

        private CityCenterProductionFactory()
        {
        }

        public Type ResultType => typeof(CityCenter);

        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner);
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
            if (!(point.Unit is Pioneer))
                throw new InvalidOperationException("city can be placed only where Pionner is");
            point.Unit.Destroy();

            return new CityCenter(owner, point);
        }
    }
}
