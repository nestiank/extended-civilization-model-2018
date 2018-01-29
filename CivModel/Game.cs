using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.IO;
using CivModel.Common;

namespace CivModel
{
    /// <summary>
    /// Represents one civ game.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// The scheme of this game.
        /// </summary>
        public IGameScheme Scheme;

        /// <summary>
        /// <see cref="Terrain"/> of this game.
        /// </summary>
        public Terrain Terrain => _terrain;
        private readonly Terrain _terrain;

        /// <summary>
        /// The players of this game.
        /// </summary>
        public IReadOnlyList<Player> Players => _players;
        private List<Player> _players = new List<Player>();

        /// <summary>
        /// The subturn number.
        /// </summary>
        /// <remarks>
        /// Subturn represents a part of turn, dedicated to each player.
        /// </remarks>
        public int SubTurnNumber { get; private set; } = 0;

        /// <summary>
        /// The turn number.
        /// </summary>
        public int TurnNumber => SubTurnNumber / Players.Count;

        /// <summary>
        /// Gets a value indicating whether this game is inside a turn.
        /// </summary>
        public bool IsInsideTurn { get; private set; } = false;

        /// <summary>
        /// Gets the index of <see cref="PlayerInTurn"/>.
        /// </summary>
        public int PlayerNumberInTurn => SubTurnNumber % Players.Count;

        /// <summary>
        /// The player who plays in this turn.
        /// </summary>
        public Player PlayerInTurn => Players[PlayerNumberInTurn];

        /// <summary>
        /// See remark section of <see cref="Guid.ParseExact(string, string)"/>.
        /// </summary>
        private const string _guidSaveFormat = "D";

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class, by creating a new game.
        /// </summary>
        /// <param name="width">The width of the <see cref="Terrain"/> of this game. It must be positive. if the value is <c>-1</c>, uses <see cref="IGameScheme.DefaultTerrainWidth"/> of the scheme.</param>
        /// <param name="height">The height of the <see cref="Terrain"/> of this game. It must be positive. if the value is <c>-1</c>, uses <see cref="IGameScheme.DefaultTerrainHeight"/> of the scheme.</param>
        /// <param name="numOfPlayer">The number of players. It must be positive. if the value is <c>-1</c>, uses <see cref="IGameScheme.DefaultNumberOfPlayers"/> of the scheme.</param>
        /// <param name="schemeFactory">The factory for <see cref="IGameScheme"/> of the game.</param>
        /// <exception cref="ArgumentNullException"><paramref name="schemeFactory"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="width"/> is not positive
        /// or
        /// <paramref name="height"/> is not positive
        /// or
        /// <paramref name="numOfPlayer"/> is not positive
        /// or
        /// parameter is not equal to default value of scheme, while scheme forces to be.
        /// </exception>
        public Game(int width, int height, int numOfPlayer, IGameSchemeFactory schemeFactory)
        {
            if (schemeFactory == null)
                throw new ArgumentNullException("schemeFactory");
            Scheme = schemeFactory.Create();

            if (width == -1)
                width = Scheme.DefaultTerrainWidth;
            if (height == -1)
                height = Scheme.DefaultTerrainWidth;
            if (numOfPlayer == -1)
                numOfPlayer = Scheme.DefaultNumberOfPlayers;

            if (width <= 0)
                throw new ArgumentException("width is not positive", "width");
            if (height <= 0)
                throw new ArgumentException("height is not positive", "height");
            if (numOfPlayer <= 0)
                throw new ArgumentException("numOfPlayer is not positive", "numOfPlayer");

            if (Scheme.OnlyDefaultTerrain)
            {
                if (width != Scheme.DefaultTerrainWidth)
                    throw new ArgumentException("parameter is not equal to default value of scheme, while scheme forces to be", "width");
                if (height != Scheme.DefaultTerrainHeight)
                    throw new ArgumentException("parameter is not equal to default value of scheme, while scheme forces to be", "height");
            }
            if (Scheme.OnlyDefaultPlayers)
            {
                if (numOfPlayer != Scheme.DefaultNumberOfPlayers)
                    throw new ArgumentException("parameter is not equal to default value of scheme, while scheme forces to be", "numOfPlayer");
            }

            _terrain = new Terrain(width, height);

            for (int i = 0; i < numOfPlayer; ++i)
            {
                _players.Add(new Player(this));
            }

            Scheme.Initialize(this, true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class, by loading a existing save file.
        /// </summary>
        /// <param name="saveFile">The path of the save file.</param>
        /// <param name="schemeFactories">the candidates of factories for <see cref="IGameScheme"/> of the game.</param>
        /// <exception cref="ArgumentNullException"><paramref name="schemeFactories"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidDataException">
        /// save file is invalid
        /// or
        /// there is no <see cref="IGameSchemeFactory"/> for this save file.
        /// </exception>
        /// <remarks>
        /// This constructor uses <see cref="File.OpenText(string)"/>.
        /// See the list of the exceptions <see cref="File.OpenText(string)"/> may throw.
        /// </remarks>
        public Game(string saveFile, IEnumerable<IGameSchemeFactory> schemeFactories)
        {
            if (schemeFactories == null)
                throw new ArgumentNullException("schemeFactory");

            int numOfPlayer;
            using (var file = File.OpenText(saveFile))
            {
                string strGuid = file.ReadLine();
                if (strGuid == null)
                    throw new InvalidDataException("save file is invalid");

                Guid guid;
                if (!Guid.TryParseExact(strGuid, _guidSaveFormat, out guid))
                    throw new InvalidDataException("save file is invalid");

                var factory = schemeFactories.Where(f => f != null && f.Guid == guid).FirstOrDefault();
                if (factory == null)
                    throw new InvalidDataException("there is no IGameSchemeFactory for this save file");

                Scheme = factory.Create();

                int[] ints;
                try
                {
                    ints = file.ReadLine()?.Split(' ').Select(str => Convert.ToInt32(str)).ToArray();
                }
                catch (Exception e) when (e is FormatException || e is OverflowException)
                {
                    throw new InvalidDataException("save file is invalid");
                }
                if (ints == null || ints.Length != 3 || ints.Count(x => x <= 0) != 0)
                    throw new InvalidDataException("save file is invalid");

                numOfPlayer = ints[0];
                _terrain = new Terrain(ints[1], ints[2]);

                for (int y = 0; y < Terrain.Height; ++y)
                {
                    string line = file.ReadLine();
                    if (line == null)
                        throw new InvalidDataException("save file is invalid");

                    for (int x = 0; x < Terrain.Width; ++x)
                    {
                        if (x >= line.Length)
                            throw new InvalidDataException("save file is invalid");

                        int idx = "POMFSTIH".IndexOf(line[x]);
                        if (idx == -1)
                            throw new InvalidDataException("save file is invalid");

                        var point = Terrain.GetPoint(x, y);
                        point.Type = (TerrainType)idx;
                    }
                }

                for (int i = 0; i < numOfPlayer; ++i)
                {
                    _players.Add(new Player(this));
                }

                while (!file.EndOfStream)
                {
                    string line = file.ReadLine();
                    if (line == "")
                        continue;
                    try
                    {
                        ints = line.Split(',').Select(str => Convert.ToInt32(str)).ToArray();
                    }
                    catch (Exception e) when (e is FormatException || e is OverflowException)
                    {
                        throw new InvalidDataException("save file is invalid");
                    }

                    if (ints.Length != 4)
                        throw new InvalidDataException("save file is invalid");
                    if (ints[0] < 0 || ints[0] >= numOfPlayer)
                        throw new InvalidDataException("save file is invalid");

                    var pos = Position.FromPhysical(ints[1], ints[2]);
                    if (!Terrain.IsValidPosition(pos))
                        throw new InvalidDataException("save file is invalid");
                    var pt = Terrain.GetPoint(pos);

                    if (ints[3] == 0)
                    {
                        var city = new CityCenter(Players[ints[0]]);
                        city.PlacedPoint = pt;
                    }
                    else
                    {
                        throw new InvalidDataException();
                    }
                }

                Scheme.Initialize(this, false);
            }
        }

        /// <summary>
        /// Saves current status of the game to the specified save file.
        /// </summary>
        /// <param name="saveFile">The path of the save file.</param>
        public void Save(string saveFile)
        {
            using (var file = File.CreateText(saveFile))
            {
                file.WriteLine(Scheme.Factory.Guid.ToString(_guidSaveFormat));

                file.WriteLine(Players.Count + " " + Terrain.Width + " " + Terrain.Height);
                for (int y = 0; y < Terrain.Height; ++y)
                {
                    for (int x = 0; x < Terrain.Width; ++x)
                    {
                        file.Write("POMFSTIH"[(int)Terrain.GetPoint(x, y).Type]);
                    }
                    file.WriteLine();
                }

                for (int i = 0; i < Players.Count; ++i)
                {
                    foreach (var city in Players[i].Cities)
                    {
                        if (city.PlacedPoint?.Position is Position pos)
                        {
                            file.WriteLine(i + "," + pos.X + "," + pos.Y + ",0");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Starts the turn.
        /// </summary>
        /// <exception cref="InvalidOperationException">this game is inside turn yet</exception>
        public void StartTurn()
        {
            if (IsInsideTurn)
                throw new InvalidOperationException("this game is inside turn yet");

            if (SubTurnNumber % Players.Count == 0)
            {
                foreach (Player p in Players)
                {
                    p.PreTurn();
                }
            }

            foreach (Player p in Players)
            {
                p.PrePlayerSubTurn(PlayerInTurn);
            }

            IsInsideTurn = true;
        }

        /// <summary>
        /// Ends the turn.
        /// </summary>
        /// <exception cref="InvalidOperationException">the turn is not started yet</exception>
        public void EndTurn()
        {
            if (!IsInsideTurn)
                throw new InvalidOperationException("the turn is not started yet");

            foreach (Player p in Players)
            {
                p.PostPlayerSubTurn(PlayerInTurn);
            }

            if ((SubTurnNumber + 1) % Players.Count == 0)
            {
                foreach (Player p in Players)
                {
                    p.PostTurn();
                }
            }

            ++SubTurnNumber;
            IsInsideTurn = false;
        }
    }
}
