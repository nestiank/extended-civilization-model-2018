using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public sealed class AncientFinnoGermaniumMine : TileBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("3F74C92A-A927-489E-AD81-D18D14BDF65B");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxHP = 30,
            DefencePower = 0,
            GoldLogistics = 20,
            LaborLogistics = 10,
            MaxHealPerTurn = 5
        };

        public AncientFinnoGermaniumMine(Player owner, Terrain.Point point, Player donator = null)
            : base(owner, Constants, point, donator)
        {
        }

        public override void PostTurn()
        {
            base.PostTurn();

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
        public ActorConstants ActorConstants => AncientFinnoGermaniumMine.Constants;

        public double TotalLaborCost => 10;
        public double LaborCapacityPerTurn => 10;
        public double TotalGoldCost => 10;
        public double GoldCapacityPerTurn => 10;

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
