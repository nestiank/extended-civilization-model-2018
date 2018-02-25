using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public sealed class FinnoEmpireCity : CityBase
    {
        public static Guid ClassGuid { get; } = new Guid("300E06FD-B656-46DC-A668-BB36C75E3086");
        public override Guid Guid => ClassGuid;

        public override double MaxHP => 500;

        public FinnoEmpireCity(Player player, Terrain.Point point) : base(player, point)
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
    }

    public class FinnoEmpireCityProductionFactory : ITileObjectProductionFactory
    {
        public static FinnoEmpireCityProductionFactory Instance => _instance.Value;
        private static Lazy<FinnoEmpireCityProductionFactory> _instance
            = new Lazy<FinnoEmpireCityProductionFactory>(() => new FinnoEmpireCityProductionFactory());

        private FinnoEmpireCityProductionFactory()
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

            return new FinnoEmpireCity(owner, point);
        }
    }
}
