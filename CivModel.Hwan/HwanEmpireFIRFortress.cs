using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public sealed class HwanEmpireFIRFortress : TileBuilding, ITileObjectObserver
    {
        public HwanEmpireFIRFortress(Player owner, Terrain.Point point, Player donator = null)
            : base(owner, typeof(HwanEmpireFIRFortress), point, donator)
        {
            owner.Game.TileObjectObservable.AddObserver(this, ObserverPriority.Model);
            if (point.Unit != null)
            {
                if (point.Unit.Owner == this.Owner)
                {
                    AboveUnit = point.Unit;
                    AboveUnit.AttackPower += 5;
                    if (!IsForceFieldOn)
                    {
                        AboveUnit.DefencePower += 5;
                        DefUpFive = true;
                    }

                    else
                        AboveUnit.DefencePower += 15;
                }
            }
        }

        protected override void OnBeforeDestroy()
        {
            Owner.Game.TileObjectObservable.RemoveObserver(this);
            base.OnBeforeDestroy();
        }

        private Unit AboveUnit = null;

        private bool isForceFieldOn = false;
        public bool IsForceFieldOn { get => isForceFieldOn; set => isForceFieldOn = value; }


        private bool DefUpFive = false;

        public void TileObjectProduced(TileObject obj) { }

        public void TileObjectPlaced(TileObject obj)
        {
            if (obj is Unit unit && unit.PlacedPoint != null
                && unit.PlacedPoint == this.PlacedPoint
                && unit.Owner == this.Owner && AboveUnit == null)
            {
                AboveUnit = unit;
                AboveUnit.AttackPower += 5;

                if (!isForceFieldOn)
                {
                    AboveUnit.DefencePower += 5;
                    DefUpFive = true;
                }

                else
                    AboveUnit.DefencePower += 15;
            }

            else if (AboveUnit != null && obj == AboveUnit && obj.PlacedPoint != this.PlacedPoint)
            {
                AboveUnit.AttackPower -= 5;

                if(DefUpFive)
                {
                    AboveUnit.DefencePower -= 5;
                    DefUpFive = false;
                }

                else
                    AboveUnit.DefencePower -= 15;

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
