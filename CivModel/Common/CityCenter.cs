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
        public ISet<IProductionFactory> AvailableProduction => _availableProduction;
        private readonly HashSet<IProductionFactory> _availableProduction = new HashSet<IProductionFactory>();

        /// <summary>
        /// The population of this city.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Population must be bigger than 1</exception>
        /// <seealso cref="Player.Population"/>
        /// <seealso cref="PopulationIncome"/>
        public double Population
        {
            get => _population;
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("Population", value, "Population must be bigger than 1");
                _population = value;
            }
        }
        private double _population = 1;

        /// <summary>
        /// The population income of this city.
        /// </summary>
        /// <seealso cref="IGameScheme.PopulationCoefficient"/>
        /// <seealso cref="IGameScheme.PopulationHappinessConstant"/>
        public double PopulationIncome => Owner.Game.Scheme.PopulationCoefficient * (Owner.Game.Scheme.PopulationHappinessConstant + Owner.Happiness);

        /// <summary>
        /// The labor per turn which this city offers.
        /// </summary>
        /// <seealso cref="Player.Labor"/>
        /// <seealso cref="IGameScheme.LaborCoefficient"/>
        /// <seealso cref="IGameScheme.LaborHappinessConstant"/>
        public double Labor =>
            Owner.Game.Scheme.LaborCoefficient
            * InteriorBuildings.Where(b => b is FactoryBuilding).Count()
            * (Owner.Game.Scheme.LaborHappinessConstant + Owner.Happiness);

        /// <summary>
        /// The list of <see cref="InteriorBuilding"/> this city owns.
        /// </summary>
        public IReadOnlyList<InteriorBuilding> InteriorBuildings => _interiorBuildings;
        private readonly List<InteriorBuilding> _interiorBuildings = new List<InteriorBuilding>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CityCenter"/> class.
        /// </summary>
        /// <param name="owner">The player who owns this city.</param>
        /// <exception cref="ArgumentNullException"><paramref name="owner"/> is <c>null</c>.</exception>
        public CityCenter(Player owner) : base(owner)
        {
            Owner.AddCityToList(this);
            _holdingAttackAct = new AttackActorAction(this, false);
        }

        /// <summary>
        /// This method is used by <see cref="InteriorBuilding.City"/>
        /// </summary>
        internal void AddBuilding(InteriorBuilding building)
        {
            if (building.City != null)
                throw new ArgumentException("building is placed already", "building");

            _interiorBuildings.Add(building);
        }

        /// <summary>
        /// This method is used by <see cref="InteriorBuilding.City"/>
        /// </summary>
        internal void RemoveBuilding(InteriorBuilding building)
        {
            if (building.City != this)
                throw new ArgumentException("building is not placed in this city", "building");

            _interiorBuildings.Remove(building);
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
            if (PlacedPoint.Value.Unit is Unit unit)
            {
                unit.Die(opposite);
            }

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

        /// <summary>
        /// Called after a turn.
        /// </summary>
        public override void PostTurn()
        {
            base.PostTurn();

            Population += PopulationIncome;
        }
    }
}
