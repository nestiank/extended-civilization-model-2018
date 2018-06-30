using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Zap
{
    public sealed class FIRFortress : TileBuilding, ITileObjectObserver
    {
        public static Guid ClassGuid { get; } = new Guid("60835460-2FDC-4507-B8F1-7822B5283541");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxHP = 30,
            GoldLogistics = 20,
            LaborLogistics = 10,
            MaxHealPerTurn = 10
        };

        public FIRFortress(Player owner, Terrain.Point point) : base(owner, Constants, point)
        {
            owner.Game.TileObjectObservable.AddObserver(this);
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

        public void TileObjectCreated(TileObject obj) { }

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

    public class FIRFortressProductionFactory : ITileObjectProductionFactory
    {
        public static FIRFortressProductionFactory Instance => _instance.Value;
        private static Lazy<FIRFortressProductionFactory> _instance
            = new Lazy<FIRFortressProductionFactory>(() => new FIRFortressProductionFactory());
        private FIRFortressProductionFactory()
        {
        }

        public Type ResultType => typeof(FIRFortress);
        public ActorConstants ActorConstants => FIRFortress.Constants;

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
            return new FIRFortress(owner, point);
        }
    }
}
