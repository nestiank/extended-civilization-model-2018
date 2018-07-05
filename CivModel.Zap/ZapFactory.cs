using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Zap
{
    public sealed class ZapFactory : TileBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("B998C018-EE38-4970-963C-E6B636DAF8A6");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxHP = 15,
            GoldLogistics = 20,
            LaborLogistics = 10,
            MaxHealPerTurn = 2
        };
        public override double ProvidedLabor => 10;

        public ZapFactory(Player owner, Terrain.Point point) : base(owner, Constants, point) { }
    }

    public class ZapFactoryProductionFactory : ITileObjectProductionFactory
    {
        public static ZapFactoryProductionFactory Instance => _instance.Value;
        private static Lazy<ZapFactoryProductionFactory> _instance
            = new Lazy<ZapFactoryProductionFactory>(() => new ZapFactoryProductionFactory());
        private ZapFactoryProductionFactory()
        {
        }

        public Type ResultType => typeof(ZapFactory);
        public ActorConstants ActorConstants => ZapFactory.Constants;

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
            return new ZapFactory(owner, point);
        }
    }
}
