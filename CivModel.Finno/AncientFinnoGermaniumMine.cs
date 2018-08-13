using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public sealed class AncientFinnoGermaniumMine : TileBuilding
    {
        public AncientFinnoGermaniumMine(Player owner, Terrain.Point point, Player donator = null)
            : base(owner, typeof(AncientFinnoGermaniumMine), point, donator)
        {
        }

        protected override void FixedPostTurn()
        {
            base.FixedPostTurn();

            int GetGold = Game.Random.Next(100);

            if (GetGold < 6)
            {
                Owner.Gold += 5;
            }

            else if (GetGold < 75)
            {
                Owner.Gold += 10;
            }

            else
            {
                Owner.Gold += 20;
            }
        }
    }

    public class AncientFinnoGermaniumMineProductionFactory : ITileBuildingProductionFactory
    {
        public static AncientFinnoGermaniumMineProductionFactory Instance => _instance.Value;
        private static Lazy<AncientFinnoGermaniumMineProductionFactory> _instance
            = new Lazy<AncientFinnoGermaniumMineProductionFactory>(() => new AncientFinnoGermaniumMineProductionFactory());
        private AncientFinnoGermaniumMineProductionFactory()
        {
        }

        public Type ResultType => typeof(AncientFinnoGermaniumMine);

        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.TileBuilding == null && production.Owner.IsAlliedWith(point.TileOwner);
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new AncientFinnoGermaniumMine(owner, point);
        }
        public TileBuilding CreateDonation(Player owner, Terrain.Point point, Player donator)
        {
            return new AncientFinnoGermaniumMine(owner, point, donator);
        }
    }
}
