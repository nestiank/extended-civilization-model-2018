using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public sealed class AncientFinnoFIRFortress : TileBuilding, ITileObjectObserver
    {

        public AncientFinnoFIRFortress(Player owner, Terrain.Point point, Player donator = null)
            : base(owner, typeof(AncientFinnoFIRFortress), point, donator)
        {
            owner.Game.TileObjectObservable.AddObserver(this, ObserverPriority.Model);
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
