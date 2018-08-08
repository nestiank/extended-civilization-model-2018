using System;
using System.Collections.Generic;
using System.Linq;
using CivObservable;

namespace CivModel
{
    /// <summary>
    /// Represents a city as <see cref="TileBuilding"/>.
    /// </summary>
    /// <seealso cref="CivModel.TileBuilding" />
    public abstract class CityBase : TileBuilding
    {
        /// <summary>
        /// The name of this city.
        /// </summary>
        /// <remarks>
        /// <see cref="Name"/> cannot have newline characters and cannot be empty string.
        /// See the list of newline characters at <see href="https://en.wikipedia.org/wiki/Newline#Unicode"/>.
        /// </remarks>
        /// <exception cref="ArgumentException">the name is invalid or already used.</exception>
        /// <seealso cref="SetCityName(string)"/>
        /// <seealso cref="TrySetCityName(string)"/>
        public override string Name => _name;
        private string _name;

        private static int _cityNamePrefix = 1;

        /// <summary>
        /// The population of this city.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Population must be bigger or equal than 1</exception>
        /// <seealso cref="Player.Population"/>
        /// <seealso cref="PopulationIncome"/>
        public double Population
        {
            get => _population;
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("Population", value, "Population must be bigger or equal than 1");
                _population = value;
            }
        }
        private double _population = 1;

        /// <summary>
        /// The population income of this city.
        /// </summary>
        /// <seealso cref="IGameConstantScheme.PopulationConstant"/>
        /// <seealso cref="IGameConstantScheme.PopulationHappinessCoefficient"/>
        public double PopulationIncome =>
            (Owner.Game.Constants.PopulationConstant + Owner.Game.Constants.PopulationHappinessCoefficient * Owner.Happiness)
            * (InteriorBuildings.Count == 0 ? 1 : InteriorBuildings.Select(b => b.PopulationCoefficient).Aggregate((a, x) => a * x));

        /// <summary>
        /// The amount of gold this building provides.
        /// </summary>
        /// <seealso cref="InteriorBuilding.ProvidedGold"/>
        /// <seealso cref="Player.GoldIncome"/>
        public override double ProvidedGold => InteriorBuildings.Select(b => b.ProvidedGold).Sum();

        /// <summary>
        /// The amount of happiness this building provides.
        /// </summary>
        /// <seealso cref="InteriorBuilding.ProvidedHappiness"/>
        /// <seealso cref="Player.HappinessIncome"/>
        public override double ProvidedHappiness => InteriorBuildings.Select(b => b.ProvidedHappiness).Sum();

        /// <summary>
        /// The labor which this city provides.
        /// </summary>
        /// <seealso cref="InteriorBuilding.ProvidedLabor"/>
        /// <seealso cref="Player.Labor"/>
        public override double ProvidedLabor => Math.Max(0, InteriorBuildings.Select(b => b.ProvidedLabor).Sum());

        /// <summary>
        /// The list of <see cref="InteriorBuilding"/> this city owns.
        /// </summary>
        public IReadOnlyList<InteriorBuilding> InteriorBuildings => _interiorBuildings;
        private readonly SafeIterationList<InteriorBuilding> _interiorBuildings = new SafeIterationList<InteriorBuilding>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CityBase"/> class.
        /// </summary>
        /// <param name="owner">The player who owns this city.</param>
        /// <param name="constants">constants of this actor.</param>
        /// <param name="point">The tile where the object will be.</param>
        /// <param name="donator">The player donated this TileBuilding. If this TileBuilding is not donated, <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="owner"/> is <c>null</c>.
        /// or
        /// <paramref name="constants"/> is <c>null</c>.
        /// </exception>
        public CityBase(Player owner, ActorConstants constants, Terrain.Point point, Player donator)
            : base(owner, constants, point, donator)
        {
            string name;
            do
            {
                name = "CityName " + _cityNamePrefix;
                ++_cityNamePrefix;
            }
            while (!TrySetCityName(name));

            Owner.BeforeLandingCity = false;
        }

        /// <summary>
        /// Sets <see cref="Name"/> of the city. A return value indicates whether the setting is succeeded.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if succeded. otherwise, <c>false</c>.</returns>
        /// <seealso cref="Name"/>
        /// <seealso cref="SetCityName(string)"/>
        public bool TrySetCityName(string value)
        {
            if (value == Name)
                return true;

            if (value == null || value == "")
                return false;

            // https://en.wikipedia.org/wiki/Newline#Unicode
            var i = value.IndexOfAny("\u000a\u000c\u000d\u0085\u2028\u2029".ToCharArray());
            if (i != -1)
                return false;

            if (!Owner.Game.UsedCityNames.Add(value))
                return false;

            if (_name != null)
                Owner.Game.UsedCityNames.Remove(_name);

            _name = value;
            return true;
        }

        /// <summary>
        /// Sets <see cref="Name"/> of the city. The behavior of this method is equal to the setter of <see cref="Name"/>.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentException">the name is invalid or already used.</exception>
        /// <seealso cref="Name"/>
        /// <seealso cref="TrySetCityName(string)"/>
        public void SetCityName(string value)
        {
            if (!TrySetCityName(value))
                throw new ArgumentException("the name is invalid or already used", "value");
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
        /// Called when [die] by <see cref="Actor.Die(Player)" />.
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

        internal override IEnumerable<IFixedEventReceiver<IFixedTurnReceiver>> FixedTurnReceiverChildren()
        {
            var super = base.FixedTurnReceiverChildren();
            foreach (var b in InteriorBuildings)
                yield return b;
            foreach (var r in super)
                yield return r;
        }

        /// <summary>
        /// Called on fixed event [post turn].
        /// </summary>
        protected override void FixedPostTurn()
        {
            base.FixedPostTurn();

            if (Owner != null)
            {
                _population += PopulationIncome;
                if (_population < 1)
                {
                    Destroy();
                }
            }
        }
    }
}
