using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CivObservable;

namespace CivModel
{
    /// <summary>
    /// Represents a building which must be built in <see cref="CityBase"/>.
    /// </summary>
    /// <seealso cref="TileBuilding"/>
    public abstract class InteriorBuilding : IProductionResult, IFixedTurnReceiver
    {
        /// <summary>
        /// The unique identifier of this class.
        /// </summary>
        public Guid Guid { get; private set; }

        /// <summary>
        /// The <see cref="Player"/> who owns this building.
        /// </summary>
        public Player Owner { get; private set; }

        /// <summary>
        /// The <see cref="Game"/> object
        /// </summary>
        public Game Game => Owner.Game;

        /// <summary>
        /// The name of this building.
        /// </summary>
        public string TextName { get; private set; }

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
        /// The amount of gold this building provides.
        /// </summary>
        /// <seealso cref="CityBase.ProvidedGold"/>
        public double ProvidedGold => 0;

        /// <summary>
        /// The amount of happiness this building provides.
        /// </summary>
        /// <seealso cref="CityBase.ProvidedHappiness"/>
        public double ProvidedHappiness => 0;

        /// <summary>
        /// The amount of labor this building provides.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">ProvidedLabor is negative</exception>
        /// <seealso cref="CityBase.ProvidedLabor"/>
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
        /// <param name="type">The concrete type of this object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="city"/> is <c>null</c>.</exception>
        public InteriorBuilding(CityBase city, Type type)
        {
            Owner = city.Owner;
            City = city ?? throw new ArgumentNullException("city");

            ApplyPrototype(Game.PrototypeLoader.GetPrototype<InteriorBuildingPrototype>(type));
        }

        private void ApplyPrototype(InteriorBuildingPrototype proto)
        {
            Guid = proto.Guid;
            TextName = proto.TextName;
            _goldLogistics = proto.GoldLogistics;
            _providedLabor = proto.ProvidedLabor;
            _basicResearchIncome = proto.ResearchIncome;
            _researchCapacity = proto.ResearchCapacity;
            PopulationCoefficient = proto.PopulationCoefficient;
        }

        /// <summary>
        /// Called when production is finished, that is, <see cref="Production.Place(Terrain.Point)" /> is succeeded.
        /// </summary>
        /// <param name="production">The <see cref="Production" /> object that produced this object.</param>
        public void OnAfterProduce(Production production)
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
            Owner = null;
        }

        /// <summary>
        /// Called before [destroy], by <see cref="Destroy"/>
        /// </summary>
        protected virtual void OnBeforeDestroy()
        {
        }

        IEnumerable<IFixedEventReceiver<IFixedTurnReceiver>> IFixedEventReceiver<IFixedTurnReceiver>.Children => null;
        IFixedTurnReceiver IFixedEventReceiver<IFixedTurnReceiver>.Receiver => this;

        /// <summary>
        /// Called on fixed event [pre turn].
        /// </summary>
        protected virtual void FixedPreTurn()
        {
        }
        void IFixedTurnReceiver.FixedPreTurn() => FixedPreTurn();

        /// <summary>
        /// Called on fixed event [after pre turn].
        /// </summary>
        protected virtual void FixedAfterPreTurn()
        {
        }
        void IFixedTurnReceiver.FixedAfterPreTurn() => FixedAfterPreTurn();

        /// <summary>
        /// Called on fixed event [post turn].
        /// </summary>
        protected virtual void FixedPostTurn()
        {
            Research += ResearchIncome;
        }
        void IFixedTurnReceiver.FixedPostTurn() => FixedPostTurn();

        /// <summary>
        /// Called on fixed event [after post turn].
        /// </summary>
        protected virtual void FixedAfterPostTurn()
        {
        }
        void IFixedTurnReceiver.FixedAfterPostTurn() => FixedAfterPostTurn();

        /// <summary>
        /// Called on fixed event [pre subturn].
        /// </summary>
        /// <param name="playerInTurn">The player which the sub turn is dedicated to.</param>
        protected virtual void FixedPreSubTurn(Player playerInTurn)
        {
        }
        void IFixedTurnReceiver.FixedPreSubTurn(Player playerInTurn) => FixedPreSubTurn(playerInTurn);

        /// <summary>
        /// Called on fixed event [after pre subturn].
        /// </summary>
        /// <param name="playerInTurn">The player which the sub turn is dedicated to.</param>
        protected virtual void FixedAfterPreSubTurn(Player playerInTurn)
        {
        }
        void IFixedTurnReceiver.FixedAfterPreSubTurn(Player playerInTurn) => FixedAfterPreSubTurn(playerInTurn);

        /// <summary>
        /// Called on fixed event [post subturn].
        /// </summary>
        /// <param name="playerInTurn">The player which the sub turn is dedicated to.</param>
        protected virtual void FixedPostSubTurn(Player playerInTurn)
        {
        }
        void IFixedTurnReceiver.FixedPostSubTurn(Player playerInTurn) => FixedPostSubTurn(playerInTurn);

        /// <summary>
        /// Called on fixed event [post subturn].
        /// </summary>
        /// <param name="playerInTurn">The player which the sub turn is dedicated to.</param>
        protected virtual void FixedAfterPostSubTurn(Player playerInTurn)
        {
        }
        void IFixedTurnReceiver.FixedAfterPostSubTurn(Player playerInTurn) => FixedAfterPostSubTurn(playerInTurn);
    }
}
