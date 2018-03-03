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
            GoldLogistics = 20,
            LaborLogistics = 10,
            MaxHealPerTurn = 10
        };

        public HwanEmpireFIRFortress(Player owner, Terrain.Point point) : base(owner, Constants, point)
        {
            owner.Game.TileObjectObservable.AddObserver(this);
        }

        protected override double CalculateDamage(double originalDamage, Actor opposite, bool isMelee, bool isSkillAttack)
        {
            AttackTo(5, opposite, opposite.DefencePower, false, true);
            return originalDamage;
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

    public class HwanEmpireFIRFortressProductionFactory : ITileObjectProductionFactory
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
            return new TileObjectProduction(this, owner);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.TileBuilding == null
                 && (point.TileOwner == production.Owner || point.TileOwner == production.Owner.Game.Players[2] || point.TileOwner == production.Owner.Game.Players[4] || point.TileOwner == production.Owner.Game.Players[6]);
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new HwanEmpireFIRFortress(owner, point);
        }
    }
}
