using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// The factory interface of <see cref="IGameScheme"/>
    /// </summary>
    public interface IGameSchemeFactory
    {
        /// <summary>
        /// The unique identifier of this factory.
        /// </summary>
        Guid Guid { get; }

        /// <summary>
        /// <see cref="Type"/> of <see cref="IGameScheme"/> object which this factory creates.
        /// </summary>
        Type SchemeType { get; }

        /// <summary>
        /// The list of dependencies of <see cref="IGameScheme"/> which this factory creates.
        /// </summary>
        IEnumerable<Guid> Dependencies { get; }

        /// <summary>
        /// The list of known <see cref="IGameSchemeFactory"/> offered by this object.
        /// </summary>
        IEnumerable<IGameSchemeFactory> KnownSchemeFactories { get; }

        /// <summary>
        /// Creates the <see cref="IGameScheme"/> object.
        /// </summary>
        /// <returns>the <see cref="IGameScheme"/> object</returns>
        IGameScheme Create();
    }

    /// <summary>
    /// The interface represents a scheme of a <see cref="Game"/>.
    /// </summary>
    public interface IGameScheme
    {
        /// <summary>
        /// The factory object of this instance.
        /// </summary>
        IGameSchemeFactory Factory { get; }
    }

    /// <summary>
    /// The interface represents <see cref="IGameScheme"/> for startup settings.
    /// This type of scheme is exclusive, that is, can be applied only once per a game.
    /// </summary>
    public interface IGameStartupScheme : IGameScheme
    {
        /// <summary>
        /// Whether the number of players must be equal to default value or not.
        /// </summary>
        /// <seealso cref="DefaultNumberOfPlayers"/>
        bool OnlyDefaultPlayers { get; }

        /// <summary>
        /// The default number of players. It must be positive.
        /// </summary>
        int DefaultNumberOfPlayers { get; }

        /// <summary>
        /// Whether <see cref="Terrain"/> size must be equal to default value or not. It must be positive.
        /// </summary>
        /// <seealso cref="DefaultTerrainWidth"/>
        /// <seealso cref="DefaultTerrainHeight"/>
        bool OnlyDefaultTerrain { get; }

        /// <summary>
        /// The default width of the <see cref="Terrain"/>. It must be positive.
        /// </summary>
        int DefaultTerrainWidth { get; }

        /// <summary>
        /// The default height of the <see cref="Terrain"/>. It must be positive.
        /// </summary>
        int DefaultTerrainHeight { get; }

        /// <summary>
        /// Initializes the game.
        /// </summary>
        /// <param name="game">The game to initialize.</param>
        /// <param name="isNewGame"><c>true</c> if initializing a new game. <c>false</c> if initializing a game loaded from a save file.</param>
        /// <exception cref="ArgumentNullException"><paramref name="game"/> is <c>null</c>.</exception>
        void InitializeGame(Game game, bool isNewGame);
    }

    /// <summary>
    /// The interface represents <see cref="IGameScheme"/> for additional objects.
    /// This type of scheme is overlappable, that is, can be applied multiple time per a game.
    /// </summary>
    public interface IGameAdditionScheme : IGameScheme
    {
        /// <summary>
        /// An additional list of <see cref="IProductionFactory"/>. This list will be added to <see cref="Player.AvailableProduction"/>.
        /// </summary>
        IEnumerable<IProductionFactory> AdditionalProductionFactory { get; }

        /// <summary>
        /// Registers <see cref="IGuidTaggedObject"/> for this scheme.
        /// </summary>
        /// <param name="game">The <see cref="Game"/> object.</param>
        void RegisterGuid(Game game);
    }

    /// <summary>
    /// The interface represents <see cref="IGameScheme"/> for game constants.
    /// This type of scheme is exclusive, that is, can be applied only once per a game.
    /// </summary>
    /// <remarks>
    /// For performance purpose, constant values are copied into <see cref="Game.Constants"/> when game starts.
    /// </remarks>
    /// <seealso cref="Game.Constants"/>
    /// <seealso cref="GameConstants"/>
    public interface IGameConstantScheme : IGameScheme
    {
        /// <summary>
        /// Coefficient for <see cref="Player.GoldIncome"/>.
        /// </summary>
        double GoldCoefficient { get; }

        /// <summary>
        /// Constant amount of <see cref="CityBase.Population"/>.
        /// </summary>
        double PopulationConstant { get; }

        /// <summary>
        /// Coefficient of <see cref="Player.Happiness"/> for <see cref="Player.Population"/>.
        /// </summary>
        double PopulationHappinessCoefficient { get; }

        /// <summary>
        /// Coefficient for <see cref="Player.HappinessIncome"/>.
        /// </summary>
        double HappinessCoefficient { get; }

        /// <summary>
        /// Coefficient of <see cref="Player.Happiness"/> for <see cref="Player.Labor"/>.
        /// </summary>
        double LaborHappinessCoefficient { get; }

        /// <summary>
        /// Coefficient of <see cref="Player.Happiness"/> for <see cref="Player.ResearchIncome"/>.
        /// </summary>
        double ResearchHappinessCoefficient { get; }

        /// <summary>
        /// Coefficient for <see cref="Player.BasicEconomicRequire"/>.
        /// </summary>
        double EconomicRequireCoefficient { get; }

        /// <summary>
        /// Constant amount of <see cref="Player.TaxRate"/> for <see cref="Player.BasicEconomicRequire"/>.
        /// </summary>
        double EconomicRequireTaxRateConstant { get; }

        /// <summary>
        /// Coefficient for <see cref="Player.BasicResearchRequire"/>.
        /// </summary>
        double ResearchRequireCoefficient { get; }
    }
}
