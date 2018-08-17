using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Zap
{
    public sealed class FIRFortress : TileBuilding, ITileObjectObserver
    {
        public FIRFortress(Player owner, Terrain.Point point, Player donator = null)
            : base(owner, typeof(FIRFortress), point, donator)
        {
            owner.Game.TileObjectObservable.AddObserver(this, ObserverPriority.Model);
        }

        protected override void OnAfterDamage(double atk, double def, double attackerDamage, double defenderDamage,
            Actor attacker, Actor defender, Player atkOwner, Player defOwner, bool isMelee, bool isSkillAttack)
        {
            if (this == defender && attacker.Owner != null)
            {
                attacker.GetDamage(5, defOwner);
            }
        }

        protected override void OnBeforeDestroy()
        {
            Owner.Game.TileObjectObservable.RemoveObserver(this);
            base.OnBeforeDestroy();
        }

        private Unit AboveUnit = null;

        public bool IsForceFieldOn = false;

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

                if (!IsForceFieldOn)
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

                if (DefUpFive)
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

    public class FIRFortressProductionFactory : ITileBuildingProductionFactory
    {
        public static FIRFortressProductionFactory Instance => _instance.Value;
        private static Lazy<FIRFortressProductionFactory> _instance
            = new Lazy<FIRFortressProductionFactory>(() => new FIRFortressProductionFactory());
        private FIRFortressProductionFactory()
        {
        }

        public Type ResultType => typeof(FIRFortress);


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
            return new FIRFortress(owner, point);
        }
        public TileBuilding CreateDonation(Player owner, Terrain.Point point, Player donator)
        {
            return new FIRFortress(owner, point, donator);
        }
    }
}
