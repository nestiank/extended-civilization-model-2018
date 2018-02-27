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
    public sealed class Player : ITurnObserver
    {
        /// <summary>
        /// The happiness of this player. This value is in [-100, 100].
        /// </summary>
        /// <seealso cref="HappinessIncome"/>
        public double Happiness
        {
            get => _happiness;
            set
            {
                if (value < -100 || value > 100)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Happiness is not in [-100, 100]");
                _happiness = value;
            }
        }
        private double _happiness = 0;

        /// <summary>
        /// The happiness income of this player.
        /// </summary>
        /// <seealso cref="IGameConstantScheme.HappinessCoefficient"/>
        public double HappinessIncome => Game.Constants.HappinessCoefficient * ((EconomicInvestmentRatio - 1) * BasicEconomicRequire);

        /// <summary>
        /// The gold of this player. This value is not negative.
        /// </summary>
        /// <seealso cref="GoldIncome"/>
        public double Gold
        {
            get => _gold;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "gold is negative");
                _gold = value;
            }
        }
        private double _gold = 0;

        /// <summary>
        /// The gold income of this player. This is not negative, and can be different from <see cref="GoldNetIncome"/>
        /// </summary>
        /// <seealso cref="GoldNetIncomeWithoutConsumption"/>
        /// <seealso cref="GoldNetIncome"/>
        /// <seealso cref="TaxRate"/>
        /// <seealso cref="IGameConstantScheme.GoldCoefficient"/>
        public double GoldIncome => Game.Constants.GoldCoefficient * Population * TaxRate;

        /// <summary>
        /// The gold net income without repair/production consumption.
        /// </summary>
        /// <seealso cref="GoldIncome"/>
        /// <seealso cref="GoldNetIncome"/>
        public double GoldNetIncomeWithoutConsumption => GoldIncome - EconomicInvestment - ResearchInvestment - CalculateLogistics().gold;

        /// <summary>
        /// The net income of gold. <see cref="EstimatedUsedGold"/> property is used for calculation.
        /// Therefore, you must call <see cref="EstimateResourceInputs"/> before use this property.
        /// </summary>
        /// <seealso cref="GoldIncome"/>
        /// <seealso cref="GoldNetIncomeWithoutConsumption"/>
        /// <seealso cref="EstimatedUsedGold"/>
        /// <seealso cref="EstimateResourceInputs"/>
        public double GoldNetIncome => GoldNetIncomeWithoutConsumption - EstimatedUsedGold;

        /// <summary>
        /// The labor per turn of this player, not controlled by <see cref="Happiness"/>.
        /// It is equal to sum of all <see cref="CityBase.Labor"/> of cities of this player.
        /// </summary>
        /// <seealso cref="LaborWithoutLogistics"/>
        /// <seealso cref="Labor"/>
        /// <seealso cref="CityBase.Labor"/>
        public double OriginalLabor => Cities.Select(city => city.Labor).Sum();

        /// <summary>
        /// The labor per turn of this player without logistics consumption.
        /// It is calculated from <see cref="OriginalLabor"/> with <see cref="Happiness"/>.
        /// </summary>
        /// <seealso cref="OriginalLabor"/>
        /// <seealso cref="Labor"/>
        public double LaborWithoutLogistics => OriginalLabor * (1 + Game.Constants.LaborHappinessCoefficient * Happiness);

        /// <summary>
        /// The labor per turn with logistics consumption.
        /// </summary>
        /// <seealso cref="LaborWithoutLogistics"/>
        public double Labor => LaborWithoutLogistics - CalculateLogistics().labor;

        /// <summary>
        /// The total basic research income per turn of this player.
        /// </summary>
        /// <seealso cref="Research"/>
        /// <seealso cref="ResearchIncome"/>
        /// <seealso cref="ResearchInvestmentRatio"/>
        /// <seealso cref="InteriorBuilding.BasicResearchIncome"/>
        public double BasicResearchIncome =>
            Cities.SelectMany(city => city.InteriorBuildings).Select(b => b.BasicResearchIncome).Sum();

        /// <summary>
        /// The total actual research income per turn of this player.
        /// </summary>
        /// <seealso cref="Research"/>
        /// <seealso cref="BasicResearchIncome"/>
        /// <seealso cref="ResearchInvestmentRatio"/>
        /// <seealso cref="InteriorBuilding.ResearchIncome"/>
        public double ResearchIncome =>
            Cities.SelectMany(city => city.InteriorBuildings).Select(b => b.ResearchIncome).Sum();

        /// <summary>
        /// The total research of this player.
        /// </summary>
        /// <seealso cref="ResearchIncome"/>
        /// <seealso cref="BasicResearchIncome"/>
        /// <seealso cref="ResearchInvestmentRatio"/>
        /// <seealso cref="InteriorBuilding.Research"/>
        public double Research =>
            Cities.SelectMany(city => city.InteriorBuildings).Select(b => b.Research).Sum();

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
        /// The ratio of real amount to basic amount of repair investment. It must be in [<c>0</c>, <c>1</c>].
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">value is not in [<c>0</c>, <c>1</c>].</exception>
        /// <seealso cref="BasicLaborForRepair"/>
        public double RepairInvestmentRatio
        {
            get => _repairInvestmentRatio;
            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "value is not in [0, 1]");
                _repairInvestmentRatio = value;
            }
        }
        private double _repairInvestmentRatio = 1;

        /// <summary>
        /// The amount of labor for repair investment.
        /// </summary>
        public double RepairInvestment => Math.Min(Labor, RepairInvestmentRatio * BasicLaborForRepair);

        /// <summary>
        /// The basic labor requirement for repair.
        /// </summary>
        public double BasicLaborForRepair => Actors.Select(actor => actor.BasicLaborForRepair).Sum();

        /// <summary>
        /// The basic economic gold requirement.
        /// </summary>
        /// <seealso cref="EconomicInvestmentRatio"/>
        public double BasicEconomicRequire => Game.Constants.EconomicRequireCoefficient * Population
            * (Game.Constants.EconomicRequireTaxRateConstant + TaxRate);

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
        public double BasicResearchRequire => Game.Constants.ResearchRequireCoefficient * BasicResearchIncome;

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
        private readonly SafeIterationCollection<Unit> _units = new SafeIterationCollection<Unit>();

        /// <summary>
        /// The list of cities of this player.
        /// </summary>
        /// <seealso cref="CityBase"/>
        public IReadOnlyList<CityBase> Cities => _cities;
        private readonly SafeIterationCollection<CityBase> _cities = new SafeIterationCollection<CityBase>();

        /// <summary>
        /// <see cref="IEnumerable{T}"/> object which contains <see cref="Actor"/> objects this player owns.
        /// </summary>
        public IEnumerable<Actor> Actors => Units.Cast<Actor>().Concat(Cities);

        /// <summary>
        /// The list of <see cref="Quest"/> which this player is <see cref="Quest.Requestee"/>.
        /// </summary>
        public IReadOnlyList<Quest> Quests => _quests;
        private readonly SafeIterationCollection<Quest> _quests = new SafeIterationCollection<Quest>();

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
        /// Whether this player is controlled by AI.
        /// </summary>
        public bool IsAIControlled
        {
            get => _aiController != null;
            set
            {
                if (value && !IsAIControlled)
                {
                    var scheme = Game.SchemeLoader.GetExclusiveScheme<IGameAIScheme>();
                    _aiController = scheme.CreateAI(this);
                }
                else if (!value && IsAIControlled)
                {
                    _aiController.Destroy();
                    _aiController = null;
                }
            }
        }
        private IAIController _aiController = null;

        private Dictionary<ISpecialResource, (object data, int count)> _specialResources
            = new Dictionary<ISpecialResource, (object, int)>();

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

            _specialResourceProxy = new SpecialResourceProxy { thiz = this };

            game.TurnObservable.AddObserver(this);
        }

        /// <summary>
        /// Let AI Controller act. This method can be asynchronous.
        /// </summary>
        /// <remarks>
        /// Since this method can be asynchronous, the model <strong>must not</strong> changed until the task is completed.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">this player does not controlled by AI</exception>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        public Task DoAITurnAction()
        {
            if (!IsAIControlled)
                throw new InvalidOperationException("this player does not controlled by AI");

            return _aiController.DoAction();
        }

        /// <summary>
        /// The indexer proxy for special resources of this player.
        /// </summary>
        /// <remarks>
        /// Usage: <code>player.SpecialResource[res]</code>
        /// </remarks>
        public SpecialResourceProxy SpecialResource => _specialResourceProxy;
        private readonly SpecialResourceProxy _specialResourceProxy;
        /// <summary>
        /// The proxy class for <see cref="SpecialResource"/>
        /// </summary>
        /// <seealso cref="SpecialResource"/>
        public class SpecialResourceProxy
        {
            internal Player thiz;
            /// <summary>
            /// Gets or sets the amount of the specified special resource.
            /// </summary>
            /// <value>
            /// The amount of the specified special resource.
            /// </value>
            /// <param name="resource">The resource.</param>
            /// <returns>The amount of the specified special resource.</returns>
            /// <exception cref="ArgumentOutOfRangeException">value - the amount of resource is out of range</exception>
            public int this[ISpecialResource resource]
            {
                get
                {
                    if (thiz._specialResources.TryGetValue(resource, out var x))
                        return x.count;
                    else
                        return 0;
                }
                set
                {
                    if (value < 0 || value > resource.MaxCount)
                        throw new ArgumentOutOfRangeException(nameof(value), value, "the amount of resource is out of range");

                    if (thiz._specialResources.TryGetValue(resource, out var x))
                    {
                        x.count = value;
                    }
                    else
                    {
                        thiz._specialResources[resource] = (null, value);
                        var data = resource.EnablePlayer(thiz);
                        thiz._specialResources[resource] = (data, value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the additional data of the specified special resource.
        /// </summary>
        /// <param name="resource">The special resource.</param>
        /// <returns>The additional data.</returns>
        public object GetSpecialResourceData(ISpecialResource resource)
        {
            if (_specialResources.TryGetValue(resource, out var x))
                return x.data;
            else
                return null;
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
        /// <seealso cref="AddTerritory(Terrain.Point)"/>
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
        /// Adds the territory of this player.
        /// </summary>
        /// <param name="pt">The tile to be in the territory.</param>
        /// <exception cref="InvalidOperationException">the owner of the tile is not this player and cannot be changed</exception>
        /// <seealso cref="TryAddTerritory(Terrain.Point)"/>
        public void AddTerritory(Terrain.Point pt)
        {
            if (!TryAddTerritory(pt))
                throw new InvalidOperationException("the owner of the tile is not this player and cannot be changed");
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

            // if (pt.TileBuilding == null) 은 사용할 수 없음
            // TileBuilding.OnAfterChangeOwner에서 RemoveTerritory할 때 실패하면 안 됌.
            if (pt.TileBuilding?.Owner == this)
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
        /// Update <see cref="Production.EstimatedLaborInputing"/>, <see cref="Production.EstimatedGoldInputing"/>,
        ///  <see cref="Actor.EstimatedLaborForRepair"/>, <see cref="EstimatedUsedLabor"/>
        ///  and <see cref="EstimatedUsedGold"/> property of this player.
        /// </summary>
        public void EstimateResourceInputs()
        {
            var labor = Labor;
            var gold = Math.Max(0, GoldNetIncomeWithoutConsumption);

            EstimatedUsedLabor = 0;
            EstimatedUsedGold = 0;

            var laborForRepair = RepairInvestment;

            foreach (var actor in Actors)
            {
                var estLabor = Math.Min(laborForRepair, actor.BasicLaborForRepair);
                actor.EstimatedLaborForRepair = estLabor;

                EstimatedUsedLabor += estLabor;

                laborForRepair -= estLabor;
                labor -= estLabor;
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
                actor.HealByRepair(actor.EstimatedLaborForRepair);
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

        // this method is used in Actor.IsStarved getter
        internal (double labor, double gold) CalculateLogistics()
        {
            double labor = LaborWithoutLogistics;
            double gold = GoldIncome;
            foreach (var actor in Actors)
            {
                if (actor.LaborLogistics <= actor.LaborLogistics)
                {
                    labor -= actor.LaborLogistics;
                    gold -= actor.GoldLogistics;
                    actor.IsStarved = false;
                }
                else
                {
                    actor.IsStarved = true;
                }
            }
            return (labor, gold);
        }
    }
}
