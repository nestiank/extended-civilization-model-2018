using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CivModel.Common;

namespace CivModel
{
    public class Player : ITurnObserver
    {
        public double Gold { get; private set; } = 0;
        public double GoldIncome => Game.GoldCoefficient * TaxRate;

        public double Happiness { get; private set; } = 100;
        public double HappinessIncome => 0;

        public double Labor => Game.LaborCoefficient * (Game.LaborHappinessConstant + Happiness);

        private double _taxRate = 1;
        public double TaxRate
        {
            get => _taxRate;
            set
            {
                if (value < 0 && value > 1)
                    throw new ArgumentException("Player.TaxRate is not in [0, 1]");
                _taxRate = value;
            }
        }

        private readonly List<Unit> _units = new List<Unit>();
        public IReadOnlyList<Unit> Units => _units;

        private readonly List<CityCenter> _cities = new List<CityCenter>();
        public IReadOnlyList<CityCenter> Cities => _cities;

        public LinkedList<Production> Production { get; } = new LinkedList<Production>();
        public LinkedList<Production> Deployment { get; } = new LinkedList<Production>();

        private readonly List<IProductionFactory> _additionalAvailableProduction = new List<IProductionFactory>();
        public List<IProductionFactory> AdditionalAvailableProduction => _additionalAvailableProduction;

        /// <summary>
        /// This property is updated by <see cref="EstimateLaborInputing"/>.
        /// You must call that function before use this property.
        /// </summary>
        public double EstimatedUsedLabor { get; private set; }

        private bool _beforeLandingCity = true;
        public bool IsDefeated => !_beforeLandingCity && Cities.Count == 0;

        private readonly Game _game;
        public Game Game => _game;

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
            Happiness += dh;
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
