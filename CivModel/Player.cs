using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// Represents a player of a game.
    /// </summary>
    /// <seealso cref="ITurnObserver"/>
    public class Player : ITurnObserver
    {
        /// <summary>
        /// The gold of this player. This value is not negative.
        /// </summary>
        /// <seealso cref="GoldIncome"/>
        public double Gold { get; private set; } = 0;

        /// <summary>
        /// The gold income of this player. This is not negative, and can be different from <see cref="GoldNetIncome"/>
        /// </summary>
        /// <seealso cref="GoldIncomeWithInvestments"/>
        /// <seealso cref="GoldNetIncome"/>
        /// <seealso cref="TaxRate"/>
        /// <seealso cref="IGameScheme.GoldCoefficient"/>
        public double GoldIncome => Game.Scheme.GoldCoefficient * Population * TaxRate;

        /// <summary>
        /// The gold income with investments.
        /// </summary>
        /// <seealso cref="GoldIncome"/>
        /// <seealso cref="GoldNetIncome"/>
        public double GoldIncomeWithInvestments => GoldIncome - EconomicInvestment - ResearchInvestment;

        /// <summary>
        /// The net income of gold. <see cref="EstimatedUsedGold"/> property is used for calculation.
        /// Therefore, you must call <see cref="EstimateInputsForProduction"/> before use this property.
        /// </summary>
        /// <seealso cref="GoldIncome"/>
        /// <seealso cref="GoldIncomeWithInvestments"/>
        /// <seealso cref="EstimatedUsedGold"/>
        /// <seealso cref="EstimateInputsForProduction"/>
        public double GoldNetIncome => GoldIncomeWithInvestments - EstimatedUsedGold;

        /// <summary>
        /// The happiness of this player. This value is in [-100, 100].
        /// </summary>
        /// <seealso cref="HappinessIncome"/>
        public double Happiness { get; private set; } = 100;

        /// <summary>
        /// The happiness income of this player.
        /// </summary>
        /// <seealso cref="IGameScheme.HappinessCoefficient"/>
        public double HappinessIncome => Game.Scheme.HappinessCoefficient * (EconomicInvestment - BasicEconomicRequire);

        /// <summary>
        /// The labor per turn of this player, not controlled by <see cref="Happiness"/>.
        /// It is equal to sum of all <see cref="CityBase.Labor"/> of cities of this player.
        /// </summary>
        /// <seealso cref="Labor"/>
        /// <seealso cref="CityBase.Labor"/>
        public double OriginalLabor => Cities.Select(city => city.Labor).Sum();

        /// <summary>
        /// The labor per turn of this player.
        /// It is calculated from <see cref="OriginalLabor"/> with <see cref="Happiness"/>.
        /// </summary>
        /// <seealso cref="OriginalLabor"/>
        /// <seealso cref="CityBase.Labor"/>
        public double Labor => OriginalLabor * (1 + Game.Scheme.LaborHappinessCoefficient * Happiness);

        /// <summary>
        /// The research per turn of this player, not controlled by <see cref="Happiness"/> and <see cref="ResearchInvestment"/>.
        /// It is equal to sum of all <see cref="CityBase.Research"/> of cities of this player.
        /// </summary>
        /// <seealso cref="Research"/>
        /// <seealso cref="CityBase.Research"/>
        /// <seealso cref="ResearchInvestment"/>
        public double OriginalResearch => Cities.Select(city => city.Research).Sum();

        /// <summary>
        /// The research per turn of this player.
        /// It is calculated from <see cref="OriginalResearch"/> with <see cref="Happiness"/> and <see cref="ResearchInvestment"/>.
        /// </summary>
        /// <seealso cref="OriginalResearch"/>
        /// <seealso cref="CityBase.Research"/>
        /// <seealso cref="ResearchInvestment"/>
        public double Research => OriginalResearch
            * (1 + Game.Scheme.ResearchHappinessCoefficient * Happiness)
            * (BasicResearchRequire != 0 ? ResearchInvestment / BasicResearchRequire : 1);

        /// <summary>
        /// The whole population which this player has. It is equal to sum of all <see cref="CityBase.Population"/> of cities of this player.
        /// </summary>
        /// <seealso cref="CityBase.Population"/>
        public double Population => Cities.Select(city => city.Population).Sum();

        /// <summary>
        /// The tax rate of this player. It affects <see cref="GoldIncome"/> and <see cref="BasicEconomicRequire"/>.
        /// This value must be in [0, 1]
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="TaxRate"/> is not in [0, 1]</exception>
        public double TaxRate
        {
            get => _taxRate;
            set
            {
                if (value < 0 && value > 1)
                    throw new ArgumentOutOfRangeException("TaxRate", value, "TaxRate is not in [0, 1]");
                _taxRate = value;
            }
        }
        private double _taxRate = 1;

        /// <summary>
        /// The basic economic gold requirement.
        /// </summary>
        /// <seealso cref="EconomicInvestment"/>
        public double BasicEconomicRequire => Game.Scheme.EconomicRequireCoefficient * Population * (Game.Scheme.EconomicRequireTaxRateConstant + TaxRate);

        /// <summary>
        /// The amount of gold for economic investment. It must be in [<c>0</c>, <c>2 * <see cref="BasicEconomicRequire"/></c>].
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <see cref="EconomicInvestment"/> is not in [<c>0</c>, <c>2 * <see cref="BasicEconomicRequire"/></c>].
        /// </exception>
        /// <seealso cref="BasicEconomicRequire"/>
        public double EconomicInvestment
        {
            get => Math.Min(_economicInvestment, 2 * BasicEconomicRequire);
            set
            {
                if (value < 0 || value > 2 * BasicEconomicRequire)
                    throw new ArgumentOutOfRangeException("EconomicInvestment", value, "EconomicInvest is not in [0, 2 * BasicEconomicRequire]");
                _economicInvestment = value;
            }
        }
        private double _economicInvestment = 0;

        /// <summary>
        /// The basic research gold requirement.
        /// </summary>
        /// <seealso cref="ResearchInvestment"/>
        public double BasicResearchRequire => Game.Scheme.ResearchRequireCoefficient * OriginalResearch;

        /// <summary>
        /// The amount of gold for research investment. It must be in [<c>0</c>, <c>2 * <see cref="BasicResearchRequire"/></c>].
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <see cref="ResearchInvestment"/> is not in [<c>0</c>, <c>2 * <see cref="BasicResearchRequire"/></c>].
        /// </exception>
        /// <seealso cref="BasicResearchRequire"/>
        public double ResearchInvestment
        {
            get => _researchInvestment;
            set
            {
                if (value < 0 || value > 2 * BasicEconomicRequire)
                    throw new ArgumentOutOfRangeException("ResearchInvestment", value, "ResearchInvestment is not in [0, 2 * BasicResearchRequire]");
                _researchInvestment = value;
            }
        }
        private double _researchInvestment = 0;

        /// <summary>
        /// The list of units of this player.
        /// </summary>
        /// <seealso cref="Unit"/>
        public IReadOnlyList<Unit> Units => _units;
        private readonly List<Unit> _units = new List<Unit>();

        /// <summary>
        /// The list of cities of this player.
        /// </summary>
        /// <seealso cref="CityBase"/>
        public IReadOnlyList<CityBase> Cities => _cities;
        private readonly List<CityBase> _cities = new List<CityBase>();

        /// <summary>
        /// The list of <see cref="Quest"/> which this player is <see cref="Quest.Requestee"/>.
        /// </summary>
        public IReadOnlyList<Quest> Quests => _quests;
        private readonly List<Quest> _quests = new List<Quest>();

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
        /// The list of available productions of this player.
        /// </summary>
        public ISet<IProductionFactory> AvailableProduction => _availableProduction;
        private readonly HashSet<IProductionFactory> _availableProduction = new HashSet<IProductionFactory>();

        /// <summary>
        /// The estimated used labor in this turn.
        /// </summary>
        /// <remarks>
        /// This property is updated by <see cref="EstimateInputsForProduction"/>.
        /// You must call that function before use this property.
        /// </remarks>
        public double EstimatedUsedLabor { get; private set; }

        /// <summary>
        /// The estimated used gold in this turn.
        /// </summary>
        /// <remarks>
        /// This property is updated by <see cref="EstimateInputsForProduction"/>.
        /// You must call that function before use this property.
        /// </remarks>
        public double EstimatedUsedGold { get; private set; }

        /// <summary>
        /// The list of tiles which this player owns as territory.
        /// </summary>
        public IReadOnlyList<Terrain.Point> Territory => _territory;
        private readonly List<Terrain.Point> _territory = new List<Terrain.Point>();

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
        /// <exception cref="ArgumentNullException"><paramref name="game"/> is <c>null</c>.</exception>
        public Player(Game game)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));

            game.AddTurnObserver(this);
        }

        // this function is used by Unit class
        internal void AddUnitToList(Unit unit)
        {
            _units.Add(unit);
        }

        // this function is used by Unit class
        internal void RemoveUnitFromList(Unit unit)
        {
            _units.Remove(unit);
        }

        /// this function is used by CityBase class
        internal void AddCityToList(CityBase city)
        {
            _beforeLandingCity = false;
            _cities.Add(city);
        }

        // this function is used by CityBase class
        internal void RemoveCityFromList(CityBase city)
        {
            _cities.Remove(city);
        }

        // this function is used by Quest class
        internal void AddQuestToList(Quest quest)
        {
            _quests.Add(quest);
        }

        /// <summary>
        /// Adds the territory of this player.
        /// </summary>
        /// <param name="pt">The tile to be in the territory.</param>
        /// <exception cref="InvalidOperationException">a <see cref="TileBuilding"/> of another player is at <paramref name="pt"/></exception>
        public void AddTerritory(Terrain.Point pt)
        {
            if (pt.TileOwner != this && pt.TileBuilding == null)
            {
                if (pt.TileOwner != null)
                    pt.TileOwner.RemoveTerritory(pt);

                pt.SetTileOwner(this);
                _territory.Add(pt);
            }
        }

        /// <summary>
        /// Removes the territory of this player.
        /// </summary>
        /// <param name="pt">The tile to be out of the territory.</param>
        /// <exception cref="ArgumentException"><paramref name="pt"/> is not in the territoriy of this player</exception>
        /// <exception cref="InvalidOperationException">the tile where a <see cref="TileBuilding"/> is cannot be removed from the territory</exception>
        public void RemoveTerritory(Terrain.Point pt)
        {
            if (pt.TileOwner != this)
                throw new ArgumentException("pt is not in the territoriy of this player", "pt");
            if (pt.TileBuilding != null)
                throw new InvalidOperationException("the tile where a TileBuilding is cannot be removed from the territory");

            _territory.Remove(pt);
            pt.SetTileOwner(null);
        }

        /// <summary>
        /// Called before a turn.
        /// </summary>
        public void PreTurn()
        {
            foreach (var unit in Units)
                unit.PreTurn();
            foreach (var city in Cities)
                city.PreTurn();
        }

        /// <summary>
        /// Called after a turn.
        /// </summary>
        public void PostTurn()
        {
            foreach (var city in Cities)
                city.PostTurn();
            foreach (var unit in Units)
                unit.PostTurn();

            // this will update GoldNetIncome
            productionProcess();

            var dg = GoldNetIncome;
            var dh = HappinessIncome;

            Gold = Math.Max(0, Gold + dg);
            Happiness = Math.Max(-100, Math.Min(100, Happiness + dh));
        }

        /// <summary>
        /// Called before a sub turn.
        /// </summary>
        /// <param name="playerInTurn">The player which the sub turn is dedicated to.</param>
        public void PrePlayerSubTurn(Player playerInTurn)
        {
            foreach (var unit in Units)
                unit.PrePlayerSubTurn(playerInTurn);
            foreach (var city in Cities)
                city.PrePlayerSubTurn(playerInTurn);
        }

        /// <summary>
        /// Called after a sub turn.
        /// </summary>
        /// <param name="playerInTurn">The player which the sub turn is dedicated to.</param>
        public void PostPlayerSubTurn(Player playerInTurn)
        {
            foreach (var city in Cities)
                city.PostPlayerSubTurn(playerInTurn);
            foreach (var unit in Units)
                unit.PostPlayerSubTurn(playerInTurn);
        }

        /// <summary>
        /// Update <see cref="Production.EstimatedLaborInputing"/> and <see cref="Production.EstimatedGoldInputing"/> property of all productions
        /// and <see cref="EstimatedUsedLabor"/> and <see cref="EstimatedUsedGold"/> property  of this player.
        /// </summary>
        public void EstimateInputsForProduction()
        {
            var labor = Labor;
            var gold = GoldIncomeWithInvestments;

            EstimatedUsedLabor = 0;
            EstimatedUsedGold = 0;

            foreach (var production in Production)
            {
                var estLabor = production.GetAvailableInputLabor(labor);
                var estGold = production.GetAvailableInputGold(gold);

                production.EstimatedLaborInputing = estLabor;
                production.EstimatedGoldInputing = estGold;

                EstimatedUsedLabor += estLabor;
                EstimatedUsedGold += estGold;

                labor -= estLabor;
                gold -= estGold;
            }
        }

        private void productionProcess()
        {
            EstimateInputsForProduction();

            for (var node = Production.First; node != null; )
            {
                var prod = node.Value;
                prod.InputResources(prod.EstimatedLaborInputing, prod.EstimatedGoldInputing);

                if (prod.IsCompleted)
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
