using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using CivObservable;

namespace CivModel
{
    /// <summary>
    /// The result of <see cref="Player.TryRemoveTerritory(Terrain.Point)"/>
    /// </summary>
    /// <seealso cref="Player.TryRemoveTerritory(Terrain.Point)"/>
    public enum RemoveTerritoryResult
    {
        /// <summary>
        /// Indicates the owner of the tile was successfully removed.
        /// </summary>
        Success,
        /// <summary>
        /// Indicates the owner of the tile is not this player.
        /// </summary>
        NotOwner,
        /// <summary>
        /// Indicates The owner of the tile cannot be changed.
        /// </summary>
        CannotRemove,
    }

    /// <summary>
    /// Represents a player of a game.
    /// </summary>
    /// <seealso cref="ITurnObserver"/>
    [DebuggerDisplay("Player (Number = {PlayerNumber})")]
    public sealed class Player : IFixedTurnReceiver, IEffectTarget
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
        public double HappinessIncome =>
            Game.Constants.HappinessCoefficient * ((EconomicInvestmentRatio - 1) * BasicEconomicRequire)
            + TileBuildings.Sum(b => b.ProvidedHappiness);

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
        public double GoldIncome =>
            Game.Constants.GoldCoefficient * Population * TaxRate
            + TileBuildings.Sum(b => b.ProvidedGold);

        /// <summary>
        /// The gold net income without repair/production consumption.
        /// </summary>
        /// <seealso cref="GoldIncome"/>
        /// <seealso cref="GoldNetIncome"/>
        public double GoldNetIncomeWithoutConsumption => CalculateLogistics().gold;

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
        /// It is equal to sum of all <see cref="CityBase.ProvidedLabor"/> of cities of this player.
        /// </summary>
        /// <seealso cref="LaborWithoutLogistics"/>
        /// <seealso cref="Labor"/>
        /// <seealso cref="CityBase.ProvidedLabor"/>
        public double OriginalLabor => TileBuildings.Sum(b => b.ProvidedLabor);

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
        public double Labor => CalculateLogistics().labor;

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
        private readonly SafeIterationList<Unit> _units = new SafeIterationList<Unit>();

        /// <summary>
        /// The list of tile buildings of this player.
        /// </summary>
        /// <seealso cref="TileBuilding"/>
        public IReadOnlyList<TileBuilding> TileBuildings => _tileBuildings;
        private readonly SafeIterationList<TileBuilding> _tileBuildings = new SafeIterationList<TileBuilding>();

        /// <summary>
        /// <see cref="IEnumerable{T}"/> object which contains cities this player owns.
        /// </summary>
        /// <seealso cref="CityBase"/>
        public IEnumerable<CityBase> Cities => TileBuildings.OfType<CityBase>();

        /// <summary>
        /// <see cref="IEnumerable{T}"/> object which contains <see cref="Actor"/> objects this player owns.
        /// </summary>
        /// <seealso cref="Actor"/>
        public IEnumerable<Actor> Actors => Units.Cast<Actor>().Concat(TileBuildings);

        /// <summary>
        /// The list of <see cref="Quest"/> which this player is <see cref="Quest.Requestee"/>.
        /// </summary>
        public IReadOnlyList<Quest> Quests => _quests;
        private readonly SafeIterationList<Quest> _quests = new SafeIterationList<Quest>();

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
        /// Whether this player is eliminated.
        /// </summary>
        public bool IsEliminated => (!BeforeLandingCity && Cities.Count() == 0) || (BeforeLandingCity && Units.Count() == 0);

        /// <summary>
        /// Whether this player has ending, that is, this player is either victoried or defeated or drawed.
        /// </summary>
        /// <seealso cref="IsVictoried"/>
        /// <seealso cref="IsDefeated"/>
        /// <seealso cref="IsDrawed"/>
        public bool HasEnding => IsVictoried || IsDefeated || IsDrawed;

        /// <summary>
        /// Whether this player is victoried.
        /// </summary>
        /// <seealso cref="VictoryCondition"/>
        public bool IsVictoried => VictoryCondition != null;

        /// <summary>
        /// The victory condition this player has achieved. If this player is not victoried yet, <c>null</c>.
        /// </summary>
        /// <seealso cref="IsVictoried"/>
        public IVictoryCondition VictoryCondition { get; private set; }

        /// <summary>
        /// Whether this player is defeated.
        /// </summary>
        /// <seealso cref="DefeatCondition"/>
        public bool IsDefeated => DefeatCondition != null;

        /// <summary>
        /// The defeat condition this player has achieved. If this player is not defeated yet, <c>null</c>.
        /// </summary>
        /// <seealso cref="IsDefeated"/>
        public IDefeatCondition DefeatCondition { get; private set; }

        /// <summary>
        /// Whether this player is drawed.
        /// </summary>
        /// <seealso cref="DrawCondition"/>
        public bool IsDrawed => DrawCondition != null;

        /// <summary>
        /// The draw condition this player has achieved. If this player is not drawed yet, <c>null</c>.
        /// </summary>
        /// <seealso cref="IsDrawed"/>
        public IDrawCondition DrawCondition { get; private set; }

        /// <summary>
        /// The list of available victories that this player can achieve.
        /// </summary>
        public IReadOnlyList<IVictoryCondition> AvailableVictories => _availableVictories;
        private readonly List<IVictoryCondition> _availableVictories = new List<IVictoryCondition>();

        /// <summary>
        /// The list of available defeats that this player can achieve.
        /// </summary>
        public IReadOnlyList<IDefeatCondition> AvailableDefeats => _availableDefeats;
        private readonly List<IDefeatCondition> _availableDefeats = new List<IDefeatCondition>();

        /// <summary>
        /// The list of available defeats that this player can achieve.
        /// </summary>
        public IReadOnlyList<IDrawCondition> AvailableDraws => _availableDraws;
        private readonly List<IDrawCondition> _availableDraws = new List<IDrawCondition>();

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

        /// <summary>
        /// The player number of this player.
        /// </summary>
        public int PlayerNumber
        {
            get
            {
                if (_playerNumber != -1)
                    return _playerNumber;

                for (int idx = 0; idx < Game.Players.Count; ++idx)
                {
                    if (Game.Players[idx] == this)
                    {
                        _playerNumber = idx;
                        return idx;
                    }
                }

                throw new InvalidOperationException("player object is invalid");
            }
        }
        private int _playerNumber = -1;

        /// <summary>
        /// The team of this player.
        /// </summary>
        public int Team
        {
            get => _team;
            set
            {
                if (value < 0 || value >= Game.TeamCount)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "team number is invalid");
                _team = value;
            }
        }
        private int _team;

        /// <summary>
        /// The name of this player.
        /// </summary>
        public string PlayerName { get; set; }

        // this property is used by CityBase class
        internal bool BeforeLandingCity { get; set; }

        private SafeIterationList<Effect> _effects = new SafeIterationList<Effect>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="game">The game which this player participates.</param>
        /// <param name="team">The team of this player.</param>
        /// <exception cref="ArgumentNullException"><paramref name="game"/> is <c>null</c>.</exception>
        public Player(Game game, int team)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));

            Team = team;

            SpecialResource = new SpecialResourceDictionary(this);
        }

        void IEffectTarget.AddEffect(Effect effect)
        {
            _effects.Add(effect);
        }

        void IEffectTarget.RemoveEffect(Effect effect)
        {
            _effects.Remove(effect);
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
        /// The dictionary for special resources of this player.
        /// </summary>
        /// <remarks>
        /// Usage: <code>player.SpecialResource[res]</code>
        /// </remarks>
        public IDictionary<ISpecialResource, int> SpecialResource { get; }

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

        /// this function is used by TileBuilding class
        internal void AddTileBuildingToList(TileBuilding city)
        {
            _tileBuildings.Add(city);
        }

        // this function is used by TileBuilding class
        internal void RemoveTileBuildingFromList(TileBuilding city)
        {
            _tileBuildings.Remove(city);
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
        /// <seealso cref="TryRemoveTerritory(Terrain.Point)"/>
        /// <seealso cref="RemoveTerritory(Terrain.Point)"/>
        public bool TryAddTerritory(Terrain.Point pt)
        {
            if (pt.TileOwner == this)
                return true;

            if (pt.TileOwner is Player other)
            {
                var rs = other.TryRemoveTerritory(pt);
                if (rs == RemoveTerritoryResult.CannotRemove)
                    return false;
            }

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
        /// <seealso cref="TryRemoveTerritory(Terrain.Point)"/>
        /// <seealso cref="RemoveTerritory(Terrain.Point)"/>
        public void AddTerritory(Terrain.Point pt)
        {
            if (!TryAddTerritory(pt))
                throw new InvalidOperationException("the owner of the tile is not this player and cannot be changed");
        }

        /// <summary>
        /// Removes the territory of this player if possible.
        /// </summary>
        /// <param name="pt">The tile to be out of the territory.</param>
        /// <returns>The result of an operation. See <see cref="RemoveTerritoryResult"/> for more information.</returns>
        /// <seealso cref="RemoveTerritoryResult"/>
        /// <seealso cref="TryAddTerritory(Terrain.Point)"/>
        /// <seealso cref="AddTerritory(Terrain.Point)"/>
        /// <seealso cref="RemoveTerritory(Terrain.Point)"/>
        public RemoveTerritoryResult TryRemoveTerritory(Terrain.Point pt)
        {
            if (pt.TileOwner != this)
                return RemoveTerritoryResult.NotOwner;

            // if (pt.TileBuilding != null) 은 사용할 수 없음
            // TileBuilding의 ctor와 OnAfterChangeOwner에서 territory를 변경할 때 실패하면 안 됌.
            if (pt.TileBuilding?.Owner == this)
                return RemoveTerritoryResult.CannotRemove;

            _territory.Remove(pt);
            pt.SetTileOwner(null);
            return RemoveTerritoryResult.Success;
        }

        /// <summary>
        /// Removes the territory of this player.
        /// </summary>
        /// <param name="pt">The tile to be out of the territory.</param>
        /// <exception cref="ArgumentException"><paramref name="pt"/> is not in the territoriy of this player</exception>
        /// <exception cref="InvalidOperationException">the tile where a <see cref="TileBuilding"/> is cannot be removed from the territory</exception>
        /// <seealso cref="TryAddTerritory(Terrain.Point)"/>
        /// <seealso cref="AddTerritory(Terrain.Point)"/>
        /// <seealso cref="TryRemoveTerritory(Terrain.Point)"/>
        public void RemoveTerritory(Terrain.Point pt)
        {
            switch (TryRemoveTerritory(pt))
            {
                case RemoveTerritoryResult.Success:
                    return;
                case RemoveTerritoryResult.NotOwner:
                    throw new ArgumentException("pt is not in the territoriy of this player", nameof(pt));
                case RemoveTerritoryResult.CannotRemove:
                    throw new InvalidOperationException("the tile where a TileBuilding is cannot be removed from the territory");
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Adds an available victory condition to this player.
        /// </summary>
        /// <param name="victory">The victory condition to add.</param>
        /// <exception cref="InvalidOperationException">the player already has ending</exception>
        /// <exception cref="ArgumentException">specified <paramref name="victory"/> is already added</exception>
        public void AddVictoryCondition(IVictoryCondition victory)
        {
            if (HasEnding)
                throw new InvalidOperationException("the player already has ending");
            if (AvailableVictories.Contains(victory))
                throw new ArgumentException("specified victory is already added", nameof(victory));

            _availableVictories.Add(victory);
        }

        /// <summary>
        /// Removes an available victory condition of this player.
        /// </summary>
        /// <param name="victory">The victory condition to remove.</param>
        /// <exception cref="InvalidOperationException">the player already has ending</exception>
        /// <exception cref="ArgumentException">specific <paramref name="victory"/> has not added</exception>
        public void RemoveVictoryCondition(IVictoryCondition victory)
        {
            if (HasEnding)
                throw new InvalidOperationException("the player already has ending");

            bool rs = _availableVictories.Remove(victory);

            if (!rs)
                throw new ArgumentException("specific victory has not added", nameof(victory));
        }

        /// <summary>
        /// Adds an available defeat condition to this player.
        /// </summary>
        /// <param name="defeat">The defeat condition to add.</param>
        /// <exception cref="InvalidOperationException">the player already has ending</exception>
        /// <exception cref="ArgumentException">specified <paramref name="defeat"/> is already added</exception>
        public void AddDefeatCondition(IDefeatCondition defeat)
        {
            if (HasEnding)
                throw new InvalidOperationException("the player already has ending");
            if (AvailableDefeats.Contains(defeat))
                throw new ArgumentException("specified defeat is already added", nameof(defeat));

            _availableDefeats.Add(defeat);
        }

        /// <summary>
        /// Removes an available defeat condition of this player.
        /// </summary>
        /// <param name="defeat">The defeat condition to remove.</param>
        /// <exception cref="InvalidOperationException">the player already has ending</exception>
        /// <exception cref="ArgumentException">specific <paramref name="defeat"/> has not added</exception>
        public void RemoveDefeatCondition(IDefeatCondition defeat)
        {
            if (HasEnding)
                throw new InvalidOperationException("the player already has ending");

            bool rs = _availableDefeats.Remove(defeat);

            if (!rs)
                throw new ArgumentException("specific defeat has not added", nameof(defeat));
        }

        /// <summary>
        /// Adds an available draw condition to this player.
        /// </summary>
        /// <param name="draw">The draw condition to add.</param>
        /// <exception cref="InvalidOperationException">the player already has ending</exception>
        /// <exception cref="ArgumentException">specified <paramref name="draw"/> is already added</exception>
        public void AddDrawCondition(IDrawCondition draw)
        {
            if (HasEnding)
                throw new InvalidOperationException("the player already has ending");
            if (AvailableDraws.Contains(draw))
                throw new ArgumentException("specified draw is already added", nameof(draw));

            _availableDraws.Add(draw);
        }

        /// <summary>
        /// Removes an available draw condition of this player.
        /// </summary>
        /// <param name="draw">The draw condition to remove.</param>
        /// <exception cref="InvalidOperationException">the player already has ending</exception>
        /// <exception cref="ArgumentException">specific <paramref name="draw"/> has not added</exception>
        public void RemoveDrawCondition(IDrawCondition draw)
        {
            if (HasEnding)
                throw new InvalidOperationException("the player already has ending");

            bool rs = _availableDraws.Remove(draw);

            if (!rs)
                throw new ArgumentException("specific draw has not added", nameof(draw));
        }

        /// <summary>
        /// Make this player achieve the specified victory.
        /// </summary>
        /// <param name="victory">The victory.</param>
        /// <exception cref="InvalidOperationException">the player already has ending</exception>
        /// <exception cref="ArgumentException">specified <paramref name="victory"/> is not available to the player</exception>
        /// <seealso cref="IsVictoried"/>
        /// <seealso cref="VictoryCondition"/>
        /// <seealso cref="Defeat(IDefeatCondition)"/>
        /// <seealso cref="Draw(IDrawCondition)"/>
        public void Victory(IVictoryCondition victory)
        {
            if (HasEnding)
                throw new InvalidOperationException("the player already has ending");
            if (!AvailableVictories.Contains(victory))
                throw new ArgumentException("specified victory is not available to the player", nameof(victory));

            VictoryCondition = victory;
            victory.DoVictory(this);

            Game.VictoryEvent.RaiseObservable(o => o.OnVictory(this, victory));
        }

        /// <summary>
        /// Make this player achieve the specified defeat.
        /// </summary>
        /// <param name="defeat">The defeat.</param>
        /// <exception cref="InvalidOperationException">the player already has ending</exception>
        /// <exception cref="ArgumentException">specified <paramref name="defeat"/> is not available to the player</exception>
        /// <seealso cref="IsDefeated"/>
        /// <seealso cref="DefeatCondition"/>
        /// <seealso cref="Victory(IVictoryCondition)"/>
        /// <seealso cref="Draw(IDrawCondition)"/>
        public void Defeat(IDefeatCondition defeat)
        {
            if (HasEnding)
                throw new InvalidOperationException("the player already has ending");
            if (!AvailableDefeats.Contains(defeat))
                throw new ArgumentException("specified defeat is not available to the player", nameof(defeat));

            DefeatCondition = defeat;
            defeat.DoDefeat(this);

            Game.VictoryEvent.RaiseObservable(o => o.OnDefeat(this, defeat));
        }

        /// <summary>
        /// Make this player achieve the specified draw.
        /// </summary>
        /// <param name="draw">The draw.</param>
        /// <exception cref="InvalidOperationException">the player already has ending</exception>
        /// <exception cref="ArgumentException">specified <paramref name="draw"/> is not available to the player</exception>
        /// <seealso cref="IsDrawed"/>
        /// <seealso cref="DrawCondition"/>
        /// <seealso cref="Victory(IVictoryCondition)"/>
        /// <seealso cref="Defeat(IDefeatCondition)"/>
        public void Draw(IDrawCondition draw)
        {
            if (HasEnding)
                throw new InvalidOperationException("the player already has ending");
            if (!AvailableDraws.Contains(draw))
                throw new ArgumentException("specified draw is not available to the player", nameof(draw));

            DrawCondition = draw;
            draw.DoDraw(this);

            Game.VictoryEvent.RaiseObservable(o => o.OnDraw(this, draw));
        }

        /// <summary>
        /// Check whether player achieves the ending condition and process it if necessary.
        /// If <see cref="HasEnding"/> is <c>true</c>, do nothing.
        /// </summary>
        /// <returns>Whether the ending process was done or not.</returns>
        public bool EndingCheck()
        {
            IDrawCondition draw = null;
            IVictoryCondition victory = null;
            IDefeatCondition defeat = null;

            if (!HasEnding)
            {
                foreach (var obj in AvailableDraws)
                {
                    if (obj.CheckDraw(this))
                    {
                        draw = obj;
                        break;
                    }
                }
                foreach (var obj in AvailableVictories)
                {
                    if (obj.CheckVictory(this))
                    {
                        victory = obj;
                        break;
                    }
                }
                foreach (var obj in AvailableDefeats)
                {
                    if (obj.CheckDefeat(this))
                    {
                        defeat = obj;
                        break;
                    }
                }
            }

            if (victory != null && defeat != null)
            {
                foreach (var obj in AvailableDraws)
                {
                    if (obj.OnBothVictoriedAndDefeated(this, victory, defeat))
                    {
                        Draw(obj);
                        return true;
                    }
                }
            }

            if (draw != null)
            {
                Draw(draw);
                return true;
            }
            if (victory != null)
            {
                Victory(victory);
                return true;
            }
            if (defeat != null)
            {
                Defeat(defeat);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Whether the specified player is not null and is allied with this player.
        /// Allied players includes the player itself. that is, <c>player.IsAlliedWith(player) == true</c>.
        /// </summary>
        /// <param name="player">The player to determine whether is allied or not.</param>
        /// <returns><c>true</c> if the specified player is allied with; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="player"/> is <c>null</c></exception>
        /// <seealso cref="IsAlliedWithOrNull(Player)"/>
        public bool IsAlliedWith(Player player)
        {
            return (player != null && player.Team == Team);
        }

        /// <summary>
        /// Whether the specified player is null or allied with this player.
        /// </summary>
        /// <param name="player">The player to determine whether is allied or not.</param>
        /// <returns><c>true</c> if the specified player is null or allied with this player; otherwise, <c>false</c>.</returns>
        /// <seealso cref="IsAlliedWith(Player)"/>
        public bool IsAlliedWithOrNull(Player player)
        {
            return (player == null || player.Team == Team);
        }

        IEnumerable<IFixedEventReceiver<IFixedTurnReceiver>> IFixedEventReceiver<IFixedTurnReceiver>.Children
        {
            // safe iteration by SafeIterationList<>
            get
            {
                foreach (var tb in _tileBuildings)
                    if (tb is CityBase)
                        yield return tb;
                foreach (var tb in _tileBuildings)
                    if (!(tb is CityBase))
                        yield return tb;
                foreach (var u in _units)
                    yield return u;
                foreach (var q in _quests)
                    yield return q;
                foreach (var e in _effects)
                    yield return e;
            }
        }
        IFixedTurnReceiver IFixedEventReceiver<IFixedTurnReceiver>.Receiver => this;

        void IFixedTurnReceiver.FixedPreTurn()
        {
        }

        void IFixedTurnReceiver.FixedAfterPreTurn()
        {
        }

        void IFixedTurnReceiver.FixedPostTurn()
        {
            // this will update GoldNetIncome
            productionProcess();

            var dg = GoldNetIncome;
            var dh = HappinessIncome;

            Gold = Math.Max(0, Gold + dg);
            Happiness = Math.Max(-100, Math.Min(100, Happiness + dh));
        }

        void IFixedTurnReceiver.FixedAfterPostTurn()
        {
        }

        void IFixedTurnReceiver.FixedPreSubTurn(Player playerInTurn)
        {
            if (playerInTurn == this)
            {
                EndingCheck();
            }
        }

        void IFixedTurnReceiver.FixedAfterPreSubTurn(Player playerInTurn)
        {
        }

        void IFixedTurnReceiver.FixedPostSubTurn(Player playerInTurn)
        {
        }

        void IFixedTurnReceiver.FixedAfterPostSubTurn(Player playerInTurn)
        {
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
            double gold = GoldIncome - EconomicInvestment - ResearchInvestment;
            foreach (var actor in Actors)
            {
                if (actor.LaborLogistics <= labor)
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
