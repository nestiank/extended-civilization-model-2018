using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class CityCenter : CityBase
    {
        public static Guid ClassGuid { get; } = new Guid("E75CDD1D-8C9C-4D9E-8310-CCD6BEBF4019");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants() {
            MaxHP = 5,
            MaxHealPerTurn = 15,
            AttackPower = 15,
            DefencePower = 21
        };

        public CityCenter(Player player, Terrain.Point point, bool isLoadFromFile)
            : base(player, Constants, point, null)
        {
            if (!isLoadFromFile)
            {
                new FactoryBuilding(this);

                foreach (var pt in PlacedPoint.Value.Adjacents())
                {
                    if (pt.HasValue)
                        Owner.TryAddTerritory(pt.Value);
                }
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

        public Type ResultType => typeof(CityCenter);
        public ActorConstants ActorConstants => CityCenter.Constants;

        public double TotalLaborCost => 5;
        public double LaborCapacityPerTurn => 2;
        public double TotalGoldCost => 5;
        public double GoldCapacityPerTurn => 2;

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

            return new CityCenter(owner, point, false);
        }
    }
}
