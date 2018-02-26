using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public sealed class AncientFinnoOctagon : TileBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("AD9DD607-2972-40F7-8596-391955621CB3");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxHP = 20,
            GoldLogistics = 20,
            FullLaborLogistics = 10,
            MaxHealPerTurn = 4
        };

        public AncientFinnoOctagon(Player owner, Terrain.Point point) : base(owner, Constants, point) { }

        public override void PostTurn()
        {
            this.RemainHP = Math.Min(20, (this.RemainHP + 4));
        }
    }

    public class AncientFinnoOctagonProductionFactory : ITileObjectProductionFactory
    {
        public static AncientFinnoOctagonProductionFactory Instance => _instance.Value;
        private static Lazy<AncientFinnoOctagonProductionFactory> _instance
            = new Lazy<AncientFinnoOctagonProductionFactory>(() => new AncientFinnoOctagonProductionFactory());
        private AncientFinnoOctagonProductionFactory()
        {
        }

        public ActorConstants ActorConstants => AncientFinnoOctagon.Constants;

        public double TotalLaborCost => 20;
        public double LaborCapacityPerTurn => 15;
        public double TotalGoldCost => 60;
        public double GoldCapacityPerTurn => 20;

        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.Unit == null
                 && point.TileBuilding is CityBase
                 && point.TileBuilding.Owner == production.Owner;
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new AncientFinnoOctagon(owner, point);
        }
    }
}
