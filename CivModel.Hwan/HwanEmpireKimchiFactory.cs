using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public sealed class HwanEmpireKimchiFactory : TileBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("E491B144-C222-42ED-8617-C59A32E902AD");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxHP = 15,
            GoldLogistics = 20,
            LaborLogistics = 10,
            MaxHealPerTurn = 2
        };

        public HwanEmpireKimchiFactory(Player owner, Terrain.Point point) : base(owner, Constants, point) { }
    }

    public class HwanEmpireKimchiFactoryProductionFactory : ITileObjectProductionFactory
    {
        public static HwanEmpireKimchiFactoryProductionFactory Instance => _instance.Value;
        private static Lazy<HwanEmpireKimchiFactoryProductionFactory> _instance
            = new Lazy<HwanEmpireKimchiFactoryProductionFactory>(() => new HwanEmpireKimchiFactoryProductionFactory());
        private HwanEmpireKimchiFactoryProductionFactory()
        {
        }

        public ActorConstants ActorConstants => HwanEmpireKimchiFactory.Constants;

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
                 && point.TileOwner == production.Owner;
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new HwanEmpireKimchiFactory(owner, point);
        }
    }
}
