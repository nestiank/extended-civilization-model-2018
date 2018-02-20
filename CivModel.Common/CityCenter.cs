using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public sealed class CityCenter : CityBase
    {
        public static Guid ClassGuid { get; } = new Guid("E75CDD1D-8C9C-4D9E-8310-CCD6BEBF4019");
        public override Guid Guid => ClassGuid;

        public CityCenter(Player player, IActorConstants constants, Terrain.Point point)
            : base(player, constants ?? new CityBaseConstants(), point)
        {
        }

        protected override void OnProcessCreation()
        {
            base.OnProcessCreation();

            new FactoryBuilding(this, null).ProcessCreation();

            foreach (var pt in PlacedPoint.Value.Adjacents())
            {
                if (pt.HasValue)
                    Owner.TryAddTerritory(pt.Value);
            }
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

        public Guid Guid => CityCenter.ClassGuid;
        public Type ProductionResultType => typeof(CityCenter);
        public IActorConstants Constants { get; } = new CityBaseConstants();

        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner, 5, 2, 5, 2);
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

            return new CityCenter(owner, Constants, point);
        }
    }
}
