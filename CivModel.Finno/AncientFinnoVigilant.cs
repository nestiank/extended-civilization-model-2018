using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public sealed class AncientFinnoVigilant : InteriorBuilding
    {
        public AncientFinnoVigilant(CityBase city) : base(city, typeof(AncientFinnoVigilant)) { }

        protected override void FixedPostTurn()
        {
            if (Game.Random.Next(5) == 0)
            {
                SendUnit();
            }

            base.FixedPostTurn();
        }

        private void SendUnit()
        {
            if (City.PlacedPoint is Terrain.Point thisPoint)
            {
                var creators = new Action<Terrain.Point>[] {
                    pt => new DecentralizedMilitary(Owner, pt).OnAfterProduce(null),
                    pt => new EMUHorseArcher(Owner, pt).OnAfterProduce(null),
                };
                var creator = creators[Game.Random.Next(creators.Length)];

                foreach (var adjacent in thisPoint.Adjacents())
                {
                    if (adjacent is Terrain.Point pt && pt.Unit == null
                        && (pt.TileBuilding == null || pt.TileBuilding.Owner == Owner))
                    {
                        creator(pt);
                        return;
                    }
                }
            }
        }
    }

    public class AncientFinnoVigilantProductionFactory : IInteriorBuildingProductionFactory
    {
        public static AncientFinnoVigilantProductionFactory Instance => _instance.Value;
        private static Lazy<AncientFinnoVigilantProductionFactory> _instance
            = new Lazy<AncientFinnoVigilantProductionFactory>(() => new AncientFinnoVigilantProductionFactory());
        private AncientFinnoVigilantProductionFactory()
        {
        }

        public Type ResultType => typeof(AncientFinnoVigilant);

        public Production Create(Player owner)
        {
            return new InteriorBuildingProduction(this, owner);
        }
        public bool IsPlacable(InteriorBuildingProduction production, CityBase city)
        {
            return true;
        }
        public InteriorBuilding CreateInteriorBuilding(CityBase city)
        {
            return new AncientFinnoVigilant(city);
        }
    }
}
