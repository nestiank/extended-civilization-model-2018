using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public sealed class HwanEmpireFIRFortress : TileBuilding, ITileObjectObserver
    {
        public static Guid ClassGuid { get; } = new Guid("B6BBF4C9-26F4-48C7-87EE-15E6F21B2DC2");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxHP = 30,
            DefencePower = 5,
            GoldLogistics = 20,
            LaborLogistics = 10,
            MaxHealPerTurn = 10
        };

        public HwanEmpireFIRFortress(Player owner, Terrain.Point point, Player donator = null)
            : base(owner, Constants, point, donator)
        {
            owner.Game.TileObjectObservable.AddObserver(this, ObserverPriority.Model);
            if (point.Unit != null)
            {
                if (point.Unit.Owner == this.Owner)
                {
                    AboveUnit = point.Unit;
                    AboveUnit.AttackPower += 5;
                    AboveUnit.DefencePower += 5;
                }
            }
        }

        protected override void OnBeforeDestroy()
        {
            Owner.Game.TileObjectObservable.RemoveObserver(this);
            base.OnBeforeDestroy();
        }

        private Unit AboveUnit = null;

        public void TileObjectProduced(TileObject obj) { }

        public void TileObjectPlaced(TileObject obj)
        {
            if (obj is Unit unit && unit.PlacedPoint != null
                && unit.PlacedPoint == this.PlacedPoint
                && unit.Owner == this.Owner && AboveUnit == null)
            {
                AboveUnit = unit;
                AboveUnit.AttackPower += 5;
                AboveUnit.DefencePower += 5;
            }

            else if (AboveUnit != null && obj == AboveUnit && obj.PlacedPoint != this.PlacedPoint)
            {
                AboveUnit.AttackPower -= 5;
                AboveUnit.DefencePower -= 5;
                AboveUnit = null;
            }
        }
    }

    public class HwanEmpireFIRFortressProductionFactory : ITileBuildingProductionFactory
    {
        public static HwanEmpireFIRFortressProductionFactory Instance => _instance.Value;
        private static Lazy<HwanEmpireFIRFortressProductionFactory> _instance
            = new Lazy<HwanEmpireFIRFortressProductionFactory>(() => new HwanEmpireFIRFortressProductionFactory());
        private HwanEmpireFIRFortressProductionFactory()
        {
        }

        public Type ResultType => typeof(HwanEmpireFIRFortress);
        public ActorConstants ActorConstants => HwanEmpireFIRFortress.Constants;

        public double TotalLaborCost => 20;
        public double LaborCapacityPerTurn => 10;
        public double TotalGoldCost => 20;
        public double GoldCapacityPerTurn => 10;

        public Production Create(Player owner)
        {
            return new TileBuildingProduction(this, owner);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.TileBuilding == null && production.Owner.IsAlliedWith(point.TileOwner);
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new HwanEmpireFIRFortress(owner, point);
        }
        public TileBuilding CreateDonation(Player owner, Terrain.Point point, Player donator)
        {
            return new HwanEmpireFIRFortress(owner, point, donator);
        }
    }
}
