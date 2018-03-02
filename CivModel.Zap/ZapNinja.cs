using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Zap
{
    public class ZapNinja : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("AB1AC3DC-3F5A-43A2-9141-8468B558D6F6");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxAP = 2,
            MaxHP = 18,
            AttackPower = 10,
            DefencePower = 3,
            GoldLogistics = 30,
            FullLaborForRepair = 2,
            BattleClassLevel = 3
        };

        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public ZapNinja(Player owner, Terrain.Point point) : base(owner, Constants, point)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
        }
    }

    public class ZapNinjaProductionFactory : ITileObjectProductionFactory
    {
        private static Lazy<ZapNinjaProductionFactory> _instance
            = new Lazy<ZapNinjaProductionFactory>(() => new ZapNinjaProductionFactory());
        public static ZapNinjaProductionFactory Instance => _instance.Value;
        private ZapNinjaProductionFactory()
        {
        }

        public Type ResultType => typeof(ZapNinja);
        public ActorConstants ActorConstants => ZapNinja.Constants;

        public double TotalLaborCost => 50;
        public double LaborCapacityPerTurn => 20;
        public double TotalGoldCost => 75;
        public double GoldCapacityPerTurn => 11;

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
            return new ZapNinja(owner, point);
        }
    }
}
