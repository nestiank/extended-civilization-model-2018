using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    /// <summary>
    /// Represents a city as <see cref="TileBuilding"/>.
    /// </summary>
    /// <seealso cref="CivModel.TileBuilding" />
    public class CityCenter : TileBuilding
    {
        /// <summary>
        /// The maximum HP. <c>0</c> if this actor is not a combattant.
        /// </summary>
        public override double MaxHP => 9;

        /// <summary>
        /// The maximum heal per turn.
        /// </summary>
        /// <seealso cref="Actor.RemainHP" />
        public override double MaxHealPerTurn => 15;

        /// <summary>
        /// The attack power.
        /// </summary>
        public override double AttackPower => 15;

        /// <summary>
        /// The defence power.
        /// </summary>
        public override double DefencePower => 21;

        /// <summary>
        /// The action performing movement. <c>null</c> if this actor cannot do.
        /// </summary>
        public override IActorAction HoldingAttackAct => _holdingAttackAct;
        private readonly IActorAction _holdingAttackAct;

        /// <summary>
        /// The list of available production from this city.
        /// </summary>
        /// <seealso cref="Player.GetAvailableProduction"/>
        public IReadOnlyList<IProductionFactory> AvailableProduction => _availableProduction;
        private List<IProductionFactory> _availableProduction = new List<IProductionFactory>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CityCenter"/> class.
        /// </summary>
        /// <param name="owner">The player who owns this city.</param>
        /// <exception cref="ArgumentNullException"><paramref name="owner"/> is <c>null</c>.</exception>
        public CityCenter(Player owner) : base(owner)
        {
            Owner.AddCityToList(this);
            _holdingAttackAct = new AttackActorAction(this, false);

            _availableProduction.Add(PioneerProductionFactory.Instance);
        }

        /// <summary>
        /// Called before [change owner], by <see cref="Actor.ChangeOwner" />.
        /// </summary>
        /// <param name="newOwner">The new owner.</param>
        protected override void OnBeforeChangeOwner(Player newOwner)
        {
            base.OnBeforeChangeOwner(newOwner);
            Owner.RemoveCityFromList(this);
            newOwner.AddCityToList(this);
        }

        /// <summary>
        /// Called before [destroy], by <see cref="Actor.Destroy" />
        /// </summary>
        protected override void OnBeforeDestroy()
        {
            Owner.RemoveCityFromList(this);
            base.OnBeforeDestroy();
        }

        /// <summary>
        /// Called when [die] by <see cref="Actor.Die(Player)" />.
        /// The default implementation calls <see cref="Actor.Destroy" />.
        /// </summary>
        /// <param name="opposite">The opposite who caused the dying of this actor. If not exists, <c>null</c>.</param>
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
