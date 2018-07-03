using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public sealed class AncientFinnoFineDustFactory : TileBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("26F24220-2B77-4E81-A985-77F3BBC77832");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxHP = 20,
            DefencePower = 2,
            GoldLogistics = 20,
            LaborLogistics = 10,
            MaxHealPerTurn = 4
        };
        public override double Labor => 20;


        public AncientFinnoFineDustFactory(Player owner, Terrain.Point point) : base(owner, Constants, point) { }

        public override void PostTurn()
        {
            base.PostTurn();
            foreach (var Player in Owner.Game.Players) // 행복도 감소
            {
                if (Player.Team != this.Owner.Team)
                {
                    if (Player.Happiness - 5 >= -100)
                        Player.Happiness -= 5;
                    else
                        Player.Happiness = -100;
                }
            }

        }
    }

    public class AncientFinnoFineDustFactoryProductionFactory : ITileObjectProductionFactory
    {
        public static AncientFinnoFineDustFactoryProductionFactory Instance => _instance.Value;
        private static Lazy<AncientFinnoFineDustFactoryProductionFactory> _instance
            = new Lazy<AncientFinnoFineDustFactoryProductionFactory>(() => new AncientFinnoFineDustFactoryProductionFactory());
        private AncientFinnoFineDustFactoryProductionFactory()
        {
        }

        public Type ResultType => typeof(AncientFinnoFineDustFactory);
        public ActorConstants ActorConstants => AncientFinnoFineDustFactory.Constants;

        public double TotalLaborCost => 20;
        public double LaborCapacityPerTurn => 10;
        public double TotalGoldCost => 20;
        public double GoldCapacityPerTurn => 10;

        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.TileBuilding == null
                 && (point.TileOwner == production.Owner || point.TileOwner == production.Owner.Game.Players[3] || point.TileOwner == production.Owner.Game.Players[5] || point.TileOwner == production.Owner.Game.Players[7]);
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new AncientFinnoFineDustFactory(owner, point);
        }
    }
}
