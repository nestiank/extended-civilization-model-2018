using System;
using System.Collections.Generic;
using System.IO;

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

        /// <summary>
        /// Called after the game is initialized.
        /// </summary>
        /// <param name="game">The <see cref="Game"/> object.</param>
        void OnAfterInitialized(Game game);
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
    /// The interface represents <see cref="IGameScheme"/> for game constants.
    /// This type of scheme is exclusive, that is, can be applied only once per a game.
    /// </summary>
    /// <remarks>
    /// This interface is dummy to mark scheme as <see cref="IGameConstants"/> provider,
    ///  although you can manually provide <see cref="IGameConstants"/> object.
    /// The <see cref="IGameConstants"/> object is retrieved from xml data.
    /// </remarks>
    /// <seealso cref="Game.Constants"/>
    /// <seealso cref="IGameConstants"/>
    public interface IGameConstantsScheme : IGameScheme
    {
        /// <summary>
        /// The <see cref="IGameConstants"/> object manually provided.
        /// If this value is <c>null</c>, the <see cref="IGameConstants"/> object is retrieved from xml data.
        /// </summary>
        IGameConstants Constants { get; }
    }

    /// <summary>
    /// The interface represents <see cref="IGameScheme"/> providing <see cref="IAIController"/>.
    /// This type of scheme is exclusive, that is, can be applied only once per a game.
    /// </summary>
    public interface IGameAIScheme : IGameScheme
    {
        /// <summary>
        /// Creates the <see cref="IAIController"/> object.
        /// </summary>
        /// <param name="player">The player for AI to control.</param>
        /// <returns>the created object</returns>
        IAIController CreateAI(Player player);
    }
}
