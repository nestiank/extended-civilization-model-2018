using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public sealed class HwanEmpireLatifundium : TileBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("10B85454-07B8-4D6A-8FF2-157870C41AF6");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxHP = 20,
            GoldLogistics = 20,
            FullLaborLogistics = 10,
            MaxHealPerTurn = 4
        };

        public HwanEmpireLatifundium(Player owner, Terrain.Point point) : base(owner, Constants, point) { }

        public override void PostTurn()
        {
            this.RemainHP = Math.Min(20, (this.RemainHP + 4));
        }
    }

    public class HwanEmpireLatifundiumProductionFactory : ITileObjectProductionFactory
    {
        public static HwanEmpireLatifundiumProductionFactory Instance => _instance.Value;
        private static Lazy<HwanEmpireLatifundiumProductionFactory> _instance
            = new Lazy<HwanEmpireLatifundiumProductionFactory>(() => new HwanEmpireLatifundiumProductionFactory());
        private HwanEmpireLatifundiumProductionFactory()
        {
        }

        public ActorConstants ActorConstants => HwanEmpireLatifundium.Constants;

        public double TotalLaborCost => 30;
        public double LaborCapacityPerTurn => 10;
        public double TotalGoldCost => 30;
        public double GoldCapacityPerTurn => 10;

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
            return new HwanEmpireLatifundium(owner, point);
        }
    }
}
