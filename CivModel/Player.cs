using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CivModel.Common;

namespace CivModel
{
    /// <summary>
    /// Represents a player of a game.
    /// </summary>
    /// <seealso cref="ITurnObserver"/>
    public class Player : ITurnObserver
    {
        /// <summary>
        /// The gold of this player.
        /// </summary>
        public double Gold { get; private set; } = 0;

        /// <summary>
        /// The gold income of this player.
        /// </summary>
        /// <seealso cref="TaxRate"/>
        /// <seealso cref="Game.GoldCoefficient"/>
        public double GoldIncome => Game.GoldCoefficient * TaxRate;

        /// <summary>
        /// The happiness of this player. This value cannot exceed <c>100</c>.
        /// </summary>
        public double Happiness { get; private set; } = 100;

        /// <summary>
        /// The happiness income of this player.
        /// </summary>
        /// <seealso cref="Game.HappinessCoefficient"/>
        public double HappinessIncome => Game.HappinessCoefficient;

        /// <summary>
        /// The labor per turn of this player.
        /// </summary>
        /// <seealso cref="Game.LaborCoefficient"/>
        /// <seealso cref="Game.LaborHappinessConstant"/>
        public double Labor => Game.LaborCoefficient * (Game.LaborHappinessConstant + Happiness);

        /// <summary>
        /// The tax rate of this player. It affects <see cref="GoldIncome"/>.
        /// </summary>
        /// <exception cref="ArgumentException"><see cref="TexRate"/> is not in [0, 1]</exception>
        public double TaxRate
        {
            get => _taxRate;
            set
            {
                if (value < 0 && value > 1)
                    throw new ArgumentException("TaxRate is not in [0, 1]");
                _taxRate = value;
            }
        }
        private double _taxRate = 1;

        /// <summary>
        /// The list of units of this player.
        /// </summary>
        /// <seealso cref="Unit"/>
        public IReadOnlyList<Unit> Units => _units;
        private readonly List<Unit> _units = new List<Unit>();

        /// <summary>
        /// The list of cities of this player.
        /// </summary>
        /// <seealso cref="CityCenter"/>
        public IReadOnlyList<CityCenter> Cities => _cities;
        private readonly List<CityCenter> _cities = new List<CityCenter>();

        /// <summary>
        /// The list of the not-finished productions of this player.
        /// </summary>
        /// <seealso cref="Deployment"/>
        public LinkedList<Production> Production { get; } = new LinkedList<Production>();

        /// <summary>
        /// The list of the ready-to-deploy productions of this player.
        /// </summary>
        /// <seealso cref="Production"/>
        public LinkedList<Production> Deployment { get; } = new LinkedList<Production>();

        /// <summary>
        /// The list of additional available productions of this player.
        /// This list will added to the calculation of <see cref="GetAvailableProduction"/>
        /// </summary>
        public List<IProductionFactory> AdditionalAvailableProduction => _additionalAvailableProduction;
        private readonly List<IProductionFactory> _additionalAvailableProduction = new List<IProductionFactory>();

        /// <summary>
        /// The estimated used labor in this turn.
        /// </summary>
        /// <remarks>
        /// This property is updated by <see cref="EstimateLaborInputing"/>.
        /// You must call that function before use this property.
        /// </remarks>
        public double EstimatedUsedLabor { get; private set; }

        /// <summary>
        /// Whether this player is defeated.
        /// </summary>
        public bool IsDefeated => !_beforeLandingCity && Cities.Count == 0;

        /// <summary>
        /// The game which this player participates.
        /// </summary>
        public Game Game => _game;
        private readonly Game _game;

        private bool _beforeLandingCity = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="game">The game which this player participates.</param>
        public Player(Game game)
        {
            _game = game;
        }

        /// <summary>
        /// this function is used by <see cref="Unit"/> class
        /// </summary>
        /// <param name="unit">unit to add</param>
        internal void AddUnitToList(Unit unit)
        {
            _units.Add(unit);
        }

        /// <summary>
        /// this function is used by <see cref="Unit"/> class
        /// </summary>
        /// <param name="unit">unit to remove</param>
        internal void RemoveUnitFromList(Unit unit)
        {
            _units.Remove(unit);
        }

        /// <summary>
        /// this function is used by <see cref="CityCenter"/> class
        /// </summary>
        /// <param name="city">city to add</param>
        internal void AddCityToList(CityCenter city)
        {
            _beforeLandingCity = false;
            _cities.Add(city);
        }

        /// <summary>
        /// this function is used by <see cref="CityCenter"/> class
        /// </summary>
        /// <param name="city">city to remove</param>
        internal void RemoveCityFromList(CityCenter city)
        {
            _cities.Remove(city);
        }

        /// <summary>
        /// Gets the list of available productions of this player.
        /// </summary>
        /// <remarks>
        /// The return value is the result of
        /// merging the result of <see cref="CityCenter.AvailableProduction"/> of all cities of this player
        /// and <see cref="AdditionalAvailableProduction"/>.
        /// </remarks>
        /// <returns>the list of available productions</returns>
        public IReadOnlyList<IProductionFactory> GetAvailableProduction()
        {
            return Cities.SelectMany(city => city.AvailableProduction).Distinct()
                .Union(AdditionalAvailableProduction).ToArray();
        }

        public void PreTurn()
        {
        }

        public void PostTurn()
        {
            var dg = GoldIncome;
            var dh = HappinessIncome;

            productionProcess();

            Gold += dg;
            Happiness = Math.Min(100, Happiness + dh);
        }

        public void PrePlayerSubTurn(Player playerInTurn)
        {
        }

        public void PostPlayerSubTurn(Player playerInTurn)
        {
        }

        /// <summary>
        /// Update <see cref="Production.EstimatedLaborInputing"/> property of all productions
        /// and <see cref="EstimatedUsedLabor"/> property  of this player.
        /// </summary>
        public void EstimateLaborInputing()
        {
            var labor = Labor;

            EstimatedUsedLabor = 0;
            foreach (var production in Production)
            {
                var input = production.GetAvailableInputLabor(labor);
                production.EstimatedLaborInputing = input;
                EstimatedUsedLabor += input;
                labor -= input;
            }
        }

        private void productionProcess()
        {
            var labor = Labor;

            for (var node = Production.First; node != null; )
            {
                labor -= node.Value.InputLabor(labor);
                if (node.Value.Completed)
                {
                    var tmp = node;
                    node = node.Next;
                    Production.Remove(tmp);
                    Deployment.AddLast(tmp.Value);
                }
                else
                {
                    node = node.Next;
                }
            }
        }
    }
}
