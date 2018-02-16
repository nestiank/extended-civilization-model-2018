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
        /// Therefore, you must call <see cref="EstimateResourceInputs"/> before use this property.
        /// </summary>
        /// <seealso cref="GoldIncome"/>
        /// <seealso cref="GoldIncomeWithInvestments"/>
        /// <seealso cref="EstimatedUsedGold"/>
        /// <seealso cref="EstimateResourceInputs"/>
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
        public double HappinessIncome => Game.Scheme.HappinessCoefficient * ((1 - EconomicInvestmentRatio) * BasicEconomicRequire);

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
        /// The research per turn of this player, not controlled by <see cref="Happiness"/> and <see cref="ResearchInvestmentRatio"/>.
        /// It is equal to sum of all <see cref="CityBase.ResearchIncome"/> of cities of this player.
        /// </summary>
        /// <seealso cref="ResearchIncome"/>
        /// <seealso cref="ResearchInvestmentRatio"/>
        /// <seealso cref="CityBase.ResearchIncome"/>
        public double OriginalResearchIncome => Cities.Select(city => city.ResearchIncome).Sum();

        /// <summary>
        /// The research per turn of this player.
        /// It is calculated from <see cref="OriginalResearchIncome"/> with <see cref="Happiness"/> and <see cref="ResearchInvestmentRatio"/>.
        /// </summary>
        /// <seealso cref="OriginalResearchIncome"/>
        /// <seealso cref="ResearchInvestmentRatio"/>
        /// <seealso cref="CityBase.ResearchIncome"/>
        public double ResearchIncome => OriginalResearchIncome * ResearchInvestmentRatio
            * (1 + Game.Scheme.ResearchHappinessCoefficient * Happiness);

        /// <summary>
        /// The total research of this player.
        /// </summary>
        /// <seealso cref="ResearchIncome"/>
        public double Research { get; private set; } = 0;

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
                    throw new ArgumentOutOfRangeException(nameof(value), value, "TaxRate is not in [0, 1]");
                _taxRate = value;
            }
        }
        private double _taxRate = 1;

        /// <summary>
        /// The ratio of real amount to basic amount of logistic investment. It must be in [<c>0</c>, <c>2</c>].
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">value is not in [<c>0</c>, <c>2</c>].</exception>
        /// <seealso cref="BasicLogisticRequire"/>
        public double LogisticInvestmentRatio
        {
            get => _logisticInvestmentRatio;
            set
            {
                if (value < 0 || value > 2)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "value is not in [0, 2]");
                _logisticInvestmentRatio = value;
            }
        }
        private double _logisticInvestmentRatio = 1;

        /// <summary>
        /// The amount of labor for logistic investment.
        /// </summary>
        public double LogisticInvestment => Math.Min(Labor, LogisticInvestmentRatio * BasicLogisticRequire);

        /// <summary>
        /// The basic logistic labor requirement.
        /// </summary>
        public double BasicLogisticRequire => Actors.Select(actor => actor.BasicLaborLogistics).Sum();

        /// <summary>
        /// The basic economic gold requirement.
        /// </summary>
        /// <seealso cref="EconomicInvestmentRatio"/>
        public double BasicEconomicRequire => Game.Scheme.EconomicRequireCoefficient * Population * (Game.Scheme.EconomicRequireTaxRateConstant + TaxRate);

        /// <summary>
        /// The ratio of real amount to basic amount of economic investment. It must be in [<c>0</c>, <c>2</c>].
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">value is not in [<c>0</c>, <c>2</c>].</exception>
        /// <seealso cref="BasicEconomicRequire"/>
        public double EconomicInvestmentRatio
        {
            get => _economicInvestmentRatio;
            set
            {
                if (value < 0 || value > 2)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "value is not in [0, 2]");
                _economicInvestmentRatio = value;
            }
        }
        private double _economicInvestmentRatio = 1;

        /// <summary>
        /// The amount of gold for economic investment.
        /// </summary>
        public double EconomicInvestment => Math.Min(GoldIncome, EconomicInvestmentRatio * BasicEconomicRequire);

        /// <summary>
        /// The basic research gold requirement.
        /// </summary>
        /// <seealso cref="ResearchInvestmentRatio"/>
        public double BasicResearchRequire => Game.Scheme.ResearchRequireCoefficient * OriginalResearchIncome;

        /// <summary>
        /// The ratio of real amount to basic amount of research investment. It must be in [<c>0</c>, <c>2</c>].
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">value is not in [<c>0</c>, <c>2</c>].</exception>
        /// <seealso cref="BasicEconomicRequire"/>
        public double ResearchInvestmentRatio
        {
            get => _researchInvestmentRatio;
            set
            {
                if (value < 0 || value > 2)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "value is not in [0, 2]");
                _researchInvestmentRatio = value;
            }
        }
        private double _researchInvestmentRatio = 1;

        /// <summary>
        /// The amount of gold for research investment.
        /// </summary>
        public double ResearchInvestment => Math.Min(GoldIncome - EconomicInvestment, ResearchInvestmentRatio * BasicResearchRequire);

        /// <summary>
        /// The list of units of this player.
        /// </summary>
        /// <seealso cref="Unit"/>
        public IReadOnlyList<Unit> Units => _units;
        private readonly SafeEnumerableCollection<Unit> _units = new SafeEnumerableCollection<Unit>();

        /// <summary>
        /// The list of cities of this player.
        /// </summary>
        /// <seealso cref="CityBase"/>
        public IReadOnlyList<CityBase> Cities => _cities;
        private readonly SafeEnumerableCollection<CityBase> _cities = new SafeEnumerableCollection<CityBase>();

        /// <summary>
        /// <see cref="IEnumerable{T}"/> object which contains <see cref="Actor"/> objects this player owns.
        /// </summary>
        public IEnumerable<Actor> Actors => Units.Cast<Actor>().Concat(Cities);

        /// <summary>
        /// The list of <see cref="Quest"/> which this player is <see cref="Quest.Requestee"/>.
        /// </summary>
        public IReadOnlyList<Quest> Quests => _quests;
        private readonly SafeEnumerableCollection<Quest> _quests = new SafeEnumerableCollection<Quest>();

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
        /// The set of available productions of this player.
        /// </summary>
        public ISet<IProductionFactory> AvailableProduction => _availableProduction;
        private readonly HashSet<IProductionFactory> _availableProduction = new HashSet<IProductionFactory>();

        /// <summary>
        /// The estimated used labor in this turn.
        /// </summary>
        /// <remarks>
        /// This property is updated by <see cref="EstimateResourceInputs"/>.
        /// You must call that function before use this property.
        /// </remarks>
        public double EstimatedUsedLabor { get; private set; }

        /// <summary>
        /// The estimated used gold in this turn.
        /// </summary>
        /// <remarks>
        /// This property is updated by <see cref="EstimateResourceInputs"/>.
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

            game.TurnObservable.AddObserver(this);
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
        /// Adds the territory of this player if possible.
        /// </summary>
        /// <param name="pt">The tile to be in the territory.</param>
        /// <returns>
        /// <c>true</c> if the owner of the tile was successfully changed or already this player.<br/>
        /// <c>false</c> if the owner of the tile is not this player and cannot be changed.
        /// </returns>
        public bool TryAddTerritory(Terrain.Point pt)
        {
            if (pt.TileOwner == this)
                return true;
            if (pt.TileOwner != null && pt.TileBuilding != null)
                return false;

            if (pt.TileOwner != null)
                pt.TileOwner.RemoveTerritory(pt);

            pt.SetTileOwner(this);
            _territory.Add(pt);
            return true;
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
            var dr = ResearchIncome;

            Gold = Math.Max(0, Gold + dg);
            Happiness = Math.Max(-100, Math.Min(100, Happiness + dh));
            Research += dr;
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
        /// Update <see cref="Production.EstimatedLaborInputing"/>, <see cref="Production.EstimatedGoldInputing"/>,
        ///  <see cref="Actor.EstimatedLaborLogicstics"/>, <see cref="EstimatedUsedLabor"/>
        ///  and <see cref="EstimatedUsedGold"/> property of this player.
        /// </summary>
        public void EstimateResourceInputs()
        {
            var labor = Labor;
            var gold = Math.Max(0, GoldIncomeWithInvestments);

            EstimatedUsedLabor = 0;
            EstimatedUsedGold = 0;

            var logistics = LogisticInvestment;

            foreach (var actor in Actors)
            {
                var estLabor = actor.GetAvailableInputLaborLogistics(logistics);
                actor.EstimatedLaborLogicstics = estLabor;

                EstimatedUsedLabor += estLabor;
                EstimatedUsedGold += actor.GoldLogistics;

                logistics -= estLabor;
                labor -= estLabor;
                gold = Math.Max(0, gold - actor.GoldLogistics);
            }

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
            EstimateResourceInputs();

            foreach (var actor in Actors)
            {
                actor.HealByLogistics(actor.EstimatedLaborLogicstics);
            }

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
