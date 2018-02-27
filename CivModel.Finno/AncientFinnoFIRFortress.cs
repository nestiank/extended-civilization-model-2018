using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public sealed class AncientFinnoFIRFortress : TileBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("F5AC55CF-C095-4525-9B87-111ED58856A2");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxHP = 30,
            GoldLogistics = 20,
            FullLaborLogistics = 10,
            MaxHealPerTurn = 10
        };


        public AncientFinnoFIRFortress(Player owner, Terrain.Point point) : base(owner, Constants, point) { }
    }

    public class AncientFinnoFIRFortressProductionFactory : ITileObjectProductionFactory
    {
        public static AncientFinnoFIRFortressProductionFactory Instance => _instance.Value;
        private static Lazy<AncientFinnoFIRFortressProductionFactory> _instance
            = new Lazy<AncientFinnoFIRFortressProductionFactory>(() => new AncientFinnoFIRFortressProductionFactory());
        private AncientFinnoFIRFortressProductionFactory()
        {
        }

        public ActorConstants ActorConstants => AncientFinnoFIRFortress.Constants;

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
            return new AncientFinnoFIRFortress(owner, point);
        }
    }
}
