using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CivModel.Common;

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
        /// Creates the <see cref="IGameScheme"/> object
        /// </summary>
        /// <returns></returns>
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
        /// Coefficient for <see cref="Player.GoldIncome"/>.
        /// </summary>
        double GoldCoefficient { get; }

        /// <summary>
        /// Coefficient for <see cref="Common.CityCenter.Population"/>.
        /// </summary>
        double PopulationCoefficient { get; }

        /// <summary>
        /// Constant amount of <see cref="Player.Happiness"/> for <see cref="Player.Population"/>.
        /// </summary>
        double PopulationHappinessConstant { get; }

        /// <summary>
        /// Coefficient for <see cref="Player.HappinessIncome"/>.
        /// </summary>
        double HappinessCoefficient { get; }

        /// <summary>
        /// Coefficient for <see cref="Player.Labor"/>.
        /// </summary>
        double LaborCoefficient { get; }

        /// <summary>
        /// Constant amount of <see cref="Player.Happiness"/> for <see cref="Player.Labor"/>.
        /// </summary>
        double LaborHappinessConstant { get; }

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

        /// <summary>
        /// Initializes the game
        /// </summary>
        /// <param name="game">The game to initialize.</param>
        /// <param name="isNewGame"><c>true</c> if initializing a new game. <c>false</c> if initializing a game loaded from a save file.</param>
        /// <exception cref="ArgumentNullException"><paramref name="game"/> is <c>null</c>.</exception>
        void InitializeGame(Game game, bool isNewGame);

        /// <summary>
        /// Initializes the city
        /// </summary>
        /// <param name="city">The city to initialize.</param>
        /// <exception cref="ArgumentNullException"><paramref name="city"/> is <c>null</c>.</exception>
        void InitializeCity(CityCenter city);
    }
}
