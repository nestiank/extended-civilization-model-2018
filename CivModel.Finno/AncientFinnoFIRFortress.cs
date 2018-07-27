using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public sealed class AncientFinnoFIRFortress : TileBuilding, ITileObjectObserver
    {
        public static Guid ClassGuid { get; } = new Guid("F5AC55CF-C095-4525-9B87-111ED58856A2");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxHP = 30,
            DefencePower = 5,
            GoldLogistics = 20,
            LaborLogistics =10,
            MaxHealPerTurn = 10
        };

        public AncientFinnoFIRFortress(Player owner, Terrain.Point point, Player donator = null)
            : base(owner, Constants, point, donator)
        {
            owner.Game.TileObjectObservable.AddObserver(this);
            if(point.Unit != null)
            {
                if(point.Unit.Owner == this.Owner)
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

    public class AncientFinnoFIRFortressProductionFactory : ITileBuildingProductionFactory
    {
        public static AncientFinnoFIRFortressProductionFactory Instance => _instance.Value;
        private static Lazy<AncientFinnoFIRFortressProductionFactory> _instance
            = new Lazy<AncientFinnoFIRFortressProductionFactory>(() => new AncientFinnoFIRFortressProductionFactory());
        private AncientFinnoFIRFortressProductionFactory()
        {
        }

        public Type ResultType => typeof(AncientFinnoFIRFortress);
        public ActorConstants ActorConstants => AncientFinnoFIRFortress.Constants;

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
            return new AncientFinnoFIRFortress(owner, point);
        }
        public TileBuilding CreateDonation(Player owner, Terrain.Point point, Player donator)
        {
            return new AncientFinnoFIRFortress(owner, point, donator);
        }
    }
}
