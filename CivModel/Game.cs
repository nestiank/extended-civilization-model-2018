using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using CivModel.Common;

namespace CivModel
{
    /// <summary>
    /// Represents one civ game.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Coefficient for <see cref="Player.GoldIncome"/>.
        /// </summary>
        public double GoldCoefficient => 1;

        /// <summary>
        /// Coefficient for PopulationIncome.
        /// </summary>
        public double PopulationCoefficient => 1;

        /// <summary>
        /// Coefficient for <see cref="Player.HappinessIncome"/>.
        /// </summary>
        public double HappinessCoefficient => 1;

        /// <summary>
        /// Coefficient for <see cref="Player.Labor"/>.
        /// </summary>
        public double LaborCoefficient => 0.1;

        /// <summary>
        /// Coefficient for <see cref="Player.Labor"/>, which works with <see cref="Player.Happiness"/>.
        /// </summary>
        public double LaborHappinessConstant => 0;

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
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        /// <param name="width">The width of the <see cref="Terrain"/> of this game. It must be positive.</param>
        /// <param name="height">The height of the <see cref="Terrain"/> of this game. It must be positive.</param>
        /// <param name="numOfPlayer">The number of player. It must be positive.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="width"/> is not positive
        /// or
        /// <paramref name="height"/> is not positive
        /// or
        /// <paramref name="numOfPlayer"/> is not positive
        /// </exception>
        public Game(int width, int height, int numOfPlayer)
        {
            if (width <= 0)
                throw new ArgumentException("width is not positive", "width");
            if (height <= 0)
                throw new ArgumentException("height is not positive", "height");
            if (numOfPlayer <= 0)
                throw new ArgumentException("numOfPlayer is not positive", "numOfPlayer");

            _terrain = new Terrain(width, height);

            for (int i = 0; i < numOfPlayer; ++i)
            {
                _players.Add(new Player(this));
            }

            var random = new Random();
            foreach (var player in Players)
            {
                Terrain.Point pt;
                do
                {
                    int x = random.Next((int)Math.Floor(Terrain.Width * 0.1),
                        (int)Math.Ceiling(Terrain.Width * 0.9));
                    int y = random.Next((int)Math.Floor(Terrain.Height * 0.1),
                        (int)Math.Ceiling(Terrain.Height * 0.9));

                    pt = Terrain.GetPoint(x, y);
                } while (pt.Unit != null);

                var pionner = new Pioneer(player);
                pionner.PlacedPoint = pt;
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
                    foreach (var unit in p.Units)
                        unit.PreTurn();
                    foreach (var city in p.Cities)
                        city.PreTurn();

                    p.PreTurn();
                }
            }

            foreach (Player p in Players)
            {
                foreach (var unit in p.Units)
                    unit.PrePlayerSubTurn(PlayerInTurn);
                foreach (var city in p.Cities)
                    city.PrePlayerSubTurn(PlayerInTurn);

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

                foreach (var unit in p.Units)
                    unit.PostPlayerSubTurn(PlayerInTurn);
                foreach (var city in p.Cities)
                    city.PostPlayerSubTurn(PlayerInTurn);
            }

            if ((SubTurnNumber + 1) % Players.Count == 0)
            {
                foreach (Player p in Players)
                {
                    p.PostTurn();

                    foreach (var unit in p.Units)
                        unit.PostTurn();
                    foreach (var city in p.Cities)
                        city.PostTurn();
                }
            }

            ++SubTurnNumber;
            IsInsideTurn = false;
        }
    }
}
