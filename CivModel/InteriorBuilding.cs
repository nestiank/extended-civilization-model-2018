using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// Represents a building which must be built in <see cref="CityBase"/>.
    /// </summary>
    /// <seealso cref="TileBuilding"/>
    public abstract class InteriorBuilding : ITurnObserver, IGuidTaggedObject
    {
        /// <summary>
        /// The unique identifier of this class.
        /// </summary>
        public abstract Guid Guid { get; }

        /// <summary>
        /// The <see cref="Player"/> who owns this building.
        /// </summary>
        public Player Owner => _owner;
        private Player _owner;

        /// <summary>
        /// The <see cref="CityBase"/> where this building is.
        /// </summary>
        public CityBase City
        {
            get => _city;
            set
            {
                if (value != _city)
                {
                    if (value != null && value.Owner != Owner)
                        throw new ArgumentException("the owner of city is different from the owner of building.", "City");

                    if (_city != null)
                    {
                        _city.RemoveBuilding(this);
                        _city = null;
                    }
                    if (value != null)
                    {
                        value.AddBuilding(this);
                        _city = value;
                    }
                }
            }
        }
        private CityBase _city = null;

        /// <summary>
        /// The original constants of this building. The actual values can be different from the values of this property.
        /// </summary>
        public InteriorBuildingConstants OriginalConstants => _originalConstants;
        private readonly InteriorBuildingConstants _originalConstants;

        /// <summary>
        /// The amount of gold logistics of this actor.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">GoldLogistics is negative</exception>
        public double GoldLogistics
        {
            get => _goldLogistics;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "GoldLogistics is negative");
                _goldLogistics = value;
            }
        }
        private double _goldLogistics;

        /// <summary>
        /// The amount of labor this building provides.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">ProvidedLabor is negative</exception>
        /// <seealso cref="CityBase.Labor"/>
        public double ProvidedLabor
        {
            get => _providedLabor;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "ProvidedLabor is negative");
                _providedLabor = value;
            }
        }
        private double _providedLabor;

        /// <summary>
        /// The amount of research capacity this building provides.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">ResearchCapacity is negative</exception>
        /// <seealso cref="BasicResearchIncome"/>
        /// <seealso cref="ResearchIncome"/>
        public double ResearchCapacity
        {
            get => _researchCapacity;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "ResearchCapacity is negative");
                _researchCapacity = value;
            }
        }
        private double _researchCapacity;

        /// <summary>
        /// The amount of basic research income per turn this building provides.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">BasicResearchIncome is negative</exception>
        /// <seealso cref="ResearchCapacity"/>
        /// <seealso cref="ResearchIncome"/>
        public double BasicResearchIncome
        {
            get => _basicResearchIncome;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "BasicResearchIncome is negative");
                _basicResearchIncome = value;
            }
        }
        private double _basicResearchIncome;

        /// <summary>
        /// The amount of research income per turn this building provides.
        /// This value is calculated with <see cref="Player.Happiness"/> and <see cref="Player.ResearchInvestmentRatio"/>.
        /// </summary>
        /// <seealso cref="ResearchCapacity"/>
        /// <seealso cref="BasicResearchIncome"/>
        public double ResearchIncome
        {
            get
            {
                var x = BasicResearchIncome* Owner.ResearchInvestmentRatio
                    * (1 + Owner.Game.Constants.ResearchHappinessCoefficient * Owner.Happiness);
                if (x + Research > ResearchCapacity)
                    return ResearchCapacity - Research;
                else
                    return x;
            }
        }

        /// <summary>
        /// Gets or sets the research.
        /// </summary>
        /// <value>
        /// The research.
        /// </value>
        /// <exception cref="System.ArgumentOutOfRangeException">value - value is not in [0, ResearchCapacity]</exception>
        public double Research
        {
            get => _research;
            set
            {
                if (value < 0 || value > ResearchCapacity)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "value is not in [0, ResearchCapacity]");
                _research = value;
            }
        }
        private double _research = 0;

        /// <summary>
        /// The population coefficient for the city where this building is.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">PopulationCoefficient is negative</exception>
        public double PopulationCoefficient { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InteriorBuilding"/> class.
        /// </summary>
        /// <param name="city">The <see cref="CityBase"/> who will own the building.</param>
        /// <param name="constants">constants of this actor.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constants"/> is <c>null</c>.
        /// or
        /// <paramref name="city"/> is <c>null</c>.
        /// </exception>
        public InteriorBuilding(CityBase city, InteriorBuildingConstants constants)
        {
            CopyConstants(constants);
            _originalConstants = constants.Clone();

            _owner = city.Owner;
            City = city ?? throw new ArgumentNullException("city");
        }

        private void CopyConstants(InteriorBuildingConstants constants)
        {
            if (constants == null)
                throw new ArgumentNullException(nameof(constants));

            _goldLogistics = constants.GoldLogistics;
            _providedLabor = constants.ProvidedLabor;
            _basicResearchIncome = constants.ResearchIncome;
            _researchCapacity = constants.ResearchCapacity;
            PopulationCoefficient = constants.PopulationCoefficient;
        }

        /// <summary>
        /// Process the logic to do at the creation of this actor.
        /// This method should not be called when this <see cref="Actor"/> object is created by loading a save file.
        /// </summary>
        /// <exception cref="InvalidOperationException"><see cref="ProcessCreation"/> has already been called</exception>
        /// <remarks>
        /// If <see cref="Actor"/> is newly created in game logic, such as <see cref="Production"/>, the creator should call this method.
        /// </remarks>
        /// <seealso cref="OnProcessCreation"/>
        public void ProcessCreation()
        {
            if (_processCreationAlreadyCalled)
                throw new InvalidOperationException("ProcessCreation has already been called");

            _processCreationAlreadyCalled = true;
            OnProcessCreation();
        }

        private bool _processCreationAlreadyCalled = false;

        /// <summary>
        /// Called when <see cref="ProcessCreation"/> is called.
        /// This method is not called when this <see cref="Actor"/> object is created by loading a save file.
        /// </summary>
        /// <seealso cref="ProcessCreation"/>
        protected virtual void OnProcessCreation()
        {
        }

        /// <summary>
        /// Destroys this building. <see cref="OnBeforeDestroy"/> is called before the building is destroyed.
        /// </summary>
        /// <remarks>
        /// <strong>postcondition</strong>:
        /// <c><see cref="TileObject.PlacedPoint"/> == null &amp;&amp; <see cref="Owner"/> == null</c>
        /// </remarks>
        public void Destroy()
        {
            OnBeforeDestroy();
            City  = null;
            _owner = null;
        }

        /// <summary>
        /// Called before [destroy], by <see cref="Destroy"/>
        /// </summary>
        protected virtual void OnBeforeDestroy()
        {
        }

        /// <summary>
        /// Called after a sub turn.
        /// </summary>
        /// <param name="playerInTurn">The player which the sub turn is dedicated to.</param>
        public virtual void PostPlayerSubTurn(Player playerInTurn)
        {
        }

        /// <summary>
        /// Called after a turn.
        /// </summary>
        public virtual void PostTurn()
        {
            Research += ResearchIncome;
        }

        /// <summary>
        /// Called before a sub turn.
        /// </summary>
        /// <param name="playerInTurn">The player which the sub turn is dedicated to.</param>
        public virtual void PrePlayerSubTurn(Player playerInTurn)
        {
        }

        /// <summary>
        /// Called before a turn.
        /// </summary>
        public virtual void PreTurn()
        {
        }
    }
}
