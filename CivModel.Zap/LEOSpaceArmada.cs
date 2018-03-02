using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Zap
{
    public class LEOSpaceArmada : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("8CA7B018-F783-4A65-9AB2-FC37A77DE6C0");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxAP = 2,
            MaxHP = 25,
            AttackPower = 8,
            DefencePower = 3,
            GoldLogistics = 20,
            FullLaborForRepair = 2,
            BattleClassLevel = 2
        };

        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public LEOSpaceArmada(Player owner, Terrain.Point point) : base(owner, Constants, point)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
        }
    }

    public class LEOSpaceArmadaProductionFactory : ITileObjectProductionFactory
    {
        private static Lazy<LEOSpaceArmadaProductionFactory> _instance
            = new Lazy<LEOSpaceArmadaProductionFactory>(() => new LEOSpaceArmadaProductionFactory());
        public static LEOSpaceArmadaProductionFactory Instance => _instance.Value;
        private LEOSpaceArmadaProductionFactory()
        {
        }

        public Type ResultType => typeof(LEOSpaceArmada);
        public ActorConstants ActorConstants => LEOSpaceArmada.Constants;

        public double TotalLaborCost => 35;
        public double LaborCapacityPerTurn => 15;
        public double TotalGoldCost => 50;
        public double GoldCapacityPerTurn => 20;

        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.Unit == null
                && point.TileBuilding is CivModel.Common.CityCenter
                && point.TileBuilding.Owner == production.Owner;
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new LEOSpaceArmada(owner,point);
        }
    }
}
