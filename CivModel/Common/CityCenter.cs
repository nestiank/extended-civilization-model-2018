using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class CityCenter : TileBuilding
    {
        public override double MaxHP => 9;
        public override double MaxHealPerTurn => 15;

        public override double AttackPower => 15;
        public override double DefencePower => 21;

        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private List<IProductionFactory> _availableProduction = new List<IProductionFactory>();
        public IReadOnlyList<IProductionFactory> AvailableProduction => _availableProduction;

        public CityCenter(Player owner) : base(owner)
        {
            Owner.AddCityToList(this);
            _holdingAttackAct = new AttackActorAction(this, false);

            _availableProduction.Add(PioneerProductionFactory.Instance);
        }

        protected override void OnChangeOwner(Player newOwner)
        {
            base.OnChangeOwner(newOwner);
            Owner.RemoveCityFromList(this);
            newOwner.AddCityToList(this);
        }

        protected override void OnDestroy()
        {
            Owner.RemoveCityFromList(this);
            base.OnDestroy();
        }

        protected override void OnDie(Player opposite)
        {
            // do not call base.OnDie(opposite) if opposite != null
            // city must be captured rather than just removed
            if (opposite != null)
            {
                ChangeOwner(opposite);
                RemainHP = MaxHP / 3;
            }
            else
            {
                base.OnDie(opposite);
            }
        }
    }
}
